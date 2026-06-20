using System.Text.Json;
using MassTransit;
using RabbitMQ.Client;
using Xango.Models.Dto;
using Xango.Services.Server.Utility;

namespace Xango.Services.Queue.Services;

public sealed class RabbitMqQueueCleanupService : IQueueCleanupService
{
	private readonly ISendEndpointProvider _sendEndpointProvider;
	private readonly ILogger<RabbitMqQueueCleanupService> _logger;

	public RabbitMqQueueCleanupService(
		ISendEndpointProvider sendEndpointProvider,
		ILogger<RabbitMqQueueCleanupService> logger)
	{
		_sendEndpointProvider = sendEndpointProvider;
		_logger = logger;
	}

	public async Task<ResponseDto> DeleteOrderFromQueue(string queueName, int orderHeaderId)
	{
		var removedCount = 0;
		var requeueMessages = new List<OrderHeaderDto>();

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

		await using var connection = await factory.CreateConnectionAsync("Xango.Services.QueueAPI.Cleanup");
		await using var channel = await connection.CreateChannelAsync();

		while (true)
		{
			var result = await channel.BasicGetAsync(queueName, autoAck: false);
			if (result == null)
			{
				break;
			}

			var orderHeader = JsonSerializer.Deserialize<OrderHeaderDto>(result.Body.Span);
			if (orderHeader == null)
			{
				await channel.BasicNackAsync(result.DeliveryTag, multiple: false, requeue: true);
				break;
			}

			await channel.BasicAckAsync(result.DeliveryTag, multiple: false);
			if (orderHeader.OrderHeaderId == orderHeaderId)
			{
				removedCount++;
			}
			else
			{
				requeueMessages.Add(orderHeader);
			}
		}

		var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{queueName}"));
		foreach (var orderHeader in requeueMessages)
		{
			await endpoint.Send(orderHeader);
		}

		_logger.LogInformation(
			"Removed {RemovedCount} messages for order {OrderHeaderId} from {QueueName}. Requeued {RequeuedCount} messages.",
			removedCount,
			orderHeaderId,
			queueName,
			requeueMessages.Count);

		return new ResponseDto
		{
			IsSuccess = true,
			Result = new
			{
				OrderHeaderId = orderHeaderId,
				QueueName = queueName,
				RemovedCount = removedCount,
				RequeuedCount = requeueMessages.Count
			}
		};
	}
}
