using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Xango.Models.Dto;
using Xango.Service.AuthenticationAPI.Client;
using Xango.Service.OrderAPI.Client;
using Xango.Services.Client.Utility;
using Xango.Services.Server.Utility;

namespace Xango.Services.MassTransit.Processor;

public sealed class OrderQueuePollingService : BackgroundService
{
	private const int DefaultPollIntervalSeconds = 5;

	private readonly IServiceScopeFactory _scopeFactory;
	private readonly ILogger<OrderQueuePollingService> _logger;
	private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);

	public OrderQueuePollingService(
		IServiceScopeFactory scopeFactory,
		ILogger<OrderQueuePollingService> logger)
	{
		_scopeFactory = scopeFactory;
		_logger = logger;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		var pollInterval = TimeSpan.FromSeconds(GetEnvironmentVariableOrDefault(
			"MESSAGE_QUEUE_POLL_INTERVAL_SECONDS",
			DefaultPollIntervalSeconds));

		var factory = new ConnectionFactory
		{
			HostName = QueueConstants.RABBITMQ_HOST(),
			UserName = QueueConstants.RABBITMQ_USER(),
			Password = QueueConstants.RABBITMQ_PASSWORD(),
			AutomaticRecoveryEnabled = true,
			NetworkRecoveryInterval = TimeSpan.FromSeconds(10),
			TopologyRecoveryEnabled = true,
			RequestedHeartbeat = TimeSpan.FromSeconds(30)
		};

		await using var connection = await factory.CreateConnectionAsync("Xango.Services.MassTransit.Processor", stoppingToken);
		await using var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);

		var queues = CreateQueueDefinitions();
		while (!stoppingToken.IsCancellationRequested)
		{
			foreach (var queue in queues)
			{
				await ProcessQueue(channel, queue, stoppingToken);
			}

			await Task.Delay(pollInterval, stoppingToken);
		}
	}

	private async Task ProcessQueue(IChannel channel, OrderQueueDefinition queue, CancellationToken cancellationToken)
	{
		var queueInfo = await channel.QueueDeclarePassiveAsync(queue.QueueName, cancellationToken);
		var messagesToCheck = queueInfo.MessageCount;

		if (messagesToCheck == 0)
		{
			return;
		}

		_logger.LogInformation(
			"Checking {MessageCount} ready messages from {QueueName}.",
			messagesToCheck,
			queue.QueueName);

		for (var i = 0U; i < messagesToCheck && !cancellationToken.IsCancellationRequested; i++)
		{
			var result = await channel.BasicGetAsync(queue.QueueName, autoAck: false, cancellationToken);
			if (result == null)
			{
				return;
			}

			var orderHeader = DeserializeOrderHeader(result.Body);
			if (orderHeader == null)
			{
				_logger.LogWarning(
					"Could not deserialize a message from {QueueName}; returning it to the queue.",
					queue.QueueName);
				await channel.BasicNackAsync(result.DeliveryTag, multiple: false, requeue: true, cancellationToken);
				continue;
			}

			if (!IsOldEnough(orderHeader, queue.PickMessageOlderThanSeconds))
			{
				await channel.BasicNackAsync(result.DeliveryTag, multiple: false, requeue: true, cancellationToken);
				continue;
			}

			await using var scope = _scopeFactory.CreateAsyncScope();
			var orderClient = scope.ServiceProvider.GetRequiredService<IOrderHttpClient>();

			await Authenticate(scope.ServiceProvider, orderClient);

			var currentOrderResponse = await orderClient.GetOrder(orderHeader.OrderHeaderId);
			var currentOrder = currentOrderResponse == null
				? null
				: DtoConverter.ToDto<OrderHeaderDto>(currentOrderResponse);

			if (currentOrder == null || currentOrder.Status != queue.ExpectedStatus)
			{
				_logger.LogInformation(
					"Removing stale message for order {OrderHeaderId} from {QueueName}. Expected status {ExpectedStatus}, current status {CurrentStatus}.",
					orderHeader.OrderHeaderId,
					queue.QueueName,
					queue.ExpectedStatus,
					currentOrder?.Status ?? "<missing>");
				await channel.BasicAckAsync(result.DeliveryTag, multiple: false, cancellationToken);
				continue;
			}

			var processed = await ProcessOrder(orderClient, queue, currentOrder);
			if (processed)
			{
				await channel.BasicAckAsync(result.DeliveryTag, multiple: false, cancellationToken);
				_logger.LogInformation(
					"Processed order {OrderHeaderId} from {QueueName}.",
					orderHeader.OrderHeaderId,
					queue.QueueName);
			}
			else
			{
				await channel.BasicNackAsync(result.DeliveryTag, multiple: false, requeue: true, cancellationToken);
				_logger.LogWarning(
					"Could not process order {OrderHeaderId} from {QueueName}; returned it to the queue.",
					orderHeader.OrderHeaderId,
					queue.QueueName);
			}
		}
	}

	private OrderHeaderDto? DeserializeOrderHeader(ReadOnlyMemory<byte> body)
	{
		try
		{
			return JsonSerializer.Deserialize<OrderHeaderDto>(body.Span, _jsonOptions);
		}
		catch (JsonException)
		{
			return null;
		}
	}

	private static bool IsOldEnough(OrderHeaderDto orderHeader, int pickMessageOlderThanSeconds)
	{
		var messageCreatedAt = orderHeader.ModifiedTime;
		if (messageCreatedAt == default)
		{
			return true;
		}

		return DateTime.Now - messageCreatedAt >= TimeSpan.FromSeconds(pickMessageOlderThanSeconds);
	}

	private static async Task Authenticate(IServiceProvider serviceProvider, IOrderHttpClient orderClient)
	{
		var authClient = serviceProvider.GetRequiredService<IAuthenticationHttpClient>();
		var response = await authClient.Login(new LoginRequestDto
		{
			UserName = Environment.GetEnvironmentVariable("AUTH_USER"),
			Password = Environment.GetEnvironmentVariable("AUTH_PASSWORD")
		});

		if (response == null || !response.IsSuccess)
		{
			throw new InvalidOperationException("Failed to authenticate with the Authentication API Client.");
		}

		var loginResult = DtoConverter.ToDto<LoginResponseDto>(response);
		if (string.IsNullOrWhiteSpace(loginResult?.Token))
		{
			throw new InvalidOperationException("Authentication API did not return a token.");
		}

		orderClient.SetToken(loginResult.Token);
	}

	private static async Task<bool> ProcessOrder(
		IOrderHttpClient orderClient,
		OrderQueueDefinition queue,
		OrderHeaderDto orderHeader)
	{
		if (queue.NextStatus == null && !queue.DeleteOrder)
		{
			return true;
		}

		ResponseDto? response;
		if (queue.DeleteOrder)
		{
			response = await orderClient.DeleteOrder(orderHeader.OrderHeaderId);
		}
		else
		{
			response = await orderClient.UpdateOrderStatus(orderHeader.OrderHeaderId, queue.NextStatus!);
		}

		return response != null && response.IsSuccess;
	}

	private static IReadOnlyList<OrderQueueDefinition> CreateQueueDefinitions()
	{
		return
		[
			new(
				QueueConstants.ORDERS_PENDING_QUEUE(),
				SD.Status_Pending,
				EnvironmentEx.GetEnvironmentVariableOrThrow<int>("QUEUE_PENDING_PICK_INTERVAL_SECONDS"),
				NextStatus: null,
				DeleteOrder: false),
			new(
				QueueConstants.ORDERS_APPROVED_QUEUE(),
				SD.Status_Approved,
				EnvironmentEx.GetEnvironmentVariableOrThrow<int>("QUEUE_APPROVED_PICK_INTERVAL_SECONDS"),
				SD.Status_ReadyForPickup,
				DeleteOrder: false),
			new(
				QueueConstants.ORDERS_READYFORPICKUP_QUEUE(),
				SD.Status_ReadyForPickup,
				EnvironmentEx.GetEnvironmentVariableOrThrow<int>("QUEUE_READYFORPICKUP_PICK_INTERVAL_SECONDS"),
				SD.Status_Completed,
				DeleteOrder: false),
			new(
				QueueConstants.ORDERS_COMPLETED_QUEUE(),
				SD.Status_Completed,
				EnvironmentEx.GetEnvironmentVariableOrThrow<int>("QUEUE_COMPLETED_PICK_INTERVAL_SECONDS"),
				SD.Status_Shipped,
				DeleteOrder: false),
			new(
				QueueConstants.ORDERS_SHIPPED_QUEUE(),
				SD.Status_Shipped,
				EnvironmentEx.GetEnvironmentVariableOrThrow<int>("QUEUE_SHIPPED_PICK_INTERVAL_SECONDS"),
				NextStatus: null,
				DeleteOrder: false),
			new(
				QueueConstants.ORDERS_CANCELLED_QUEUE(),
				SD.Status_Cancelled,
				EnvironmentEx.GetEnvironmentVariableOrThrow<int>("QUEUE_CANCELLED_PICK_INTERVAL_SECONDS"),
				NextStatus: null,
				DeleteOrder: true)
		];
	}

	private sealed record OrderQueueDefinition(
		string QueueName,
		string ExpectedStatus,
		int PickMessageOlderThanSeconds,
		string? NextStatus,
		bool DeleteOrder);

	private static int GetEnvironmentVariableOrDefault(string name, int defaultValue)
	{
		var value = Environment.GetEnvironmentVariable(name);
		return int.TryParse(value, out var parsedValue)
			? parsedValue
			: defaultValue;
	}
}
