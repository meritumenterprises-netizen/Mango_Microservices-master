using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xango.Models.Dto;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using IModel = RabbitMQ.Client.IModel;
using IConnectionFactory = RabbitMQ.Client.IConnectionFactory;
using Xango.Services.RabbitMQ.Utility;
using Xango.Services.Client.Utility;
using Microsoft.Extensions.DependencyInjection;

using static Xango.Services.Queue.Processor.QueueMessage;

namespace Xango.Services.Queue.Processor
{
	internal class RabbitMqReader
	{
		private readonly IServiceProvider _serviceProvider;

		private readonly DateTime _startTime;

		public RabbitMqReader(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
			_startTime = DateTime.Now;
		}

		public async Task StartAsync()
		{
			Console.WriteLine("[QueueMessageProcessor] Starting Queue Message Processor...");

			var cts = new CancellationTokenSource();
			var token = cts.Token;

			try
			{
				var processors = new QueueMessageProcessorBase[]
				{
			new OrdersCancelledProcessor(_serviceProvider, cts)
			{
				PickMessageOlderThanSeconds = EnvironmentEx.GetEnvironmentVariableOrThrow<int>("QUEUE_CANCELLED_PICK_INTERVAL_SECONDS"),
				CheckQueueEverySeconds = EnvironmentEx.GetEnvironmentVariableOrThrow<int>("QUEUE_CANCELLED_INTERVAL_SECONDS")
			},
			new OrdersReadyForPickupProcessor(_serviceProvider, cts)
			{
				PickMessageOlderThanSeconds = EnvironmentEx.GetEnvironmentVariableOrThrow<int>("QUEUE_READYFORPICKUP_PICK_INTERVAL_SECONDS"),
				CheckQueueEverySeconds = EnvironmentEx.GetEnvironmentVariableOrThrow<int>("QUEUE_READYFORPICKUP_INTERVAL_SECONDS")
			},
			new OrdersApprovedProcessor(_serviceProvider, cts)
			{
				PickMessageOlderThanSeconds = EnvironmentEx.GetEnvironmentVariableOrThrow<int>("QUEUE_APPROVED_PICK_INTERVAL_SECONDS"),
				CheckQueueEverySeconds = EnvironmentEx.GetEnvironmentVariableOrThrow<int>("QUEUE_APPROVED_INTERVAL_SECONDS")
			},
			new OrdersPendingProcessor(_serviceProvider, cts)
			{
				PickMessageOlderThanSeconds = EnvironmentEx.GetEnvironmentVariableOrThrow<int>("QUEUE_PENDING_PICK_INTERVAL_SECONDS"),
				CheckQueueEverySeconds = EnvironmentEx.GetEnvironmentVariableOrThrow<int>("QUEUE_PENDING_INTERVAL_SECONDS")
			},
			new OrdersCompletedProcessor(_serviceProvider, cts)
			{
				PickMessageOlderThanSeconds = EnvironmentEx.GetEnvironmentVariableOrThrow<int>("QUEUE_COMPLETED_PICK_INTERVAL_SECONDS"),
				CheckQueueEverySeconds = EnvironmentEx.GetEnvironmentVariableOrThrow<int>("QUEUE_COMPLETED_INTERVAL_SECONDS")
			},
			new OrdersShippedProcessor(_serviceProvider, cts)
			{
				PickMessageOlderThanSeconds = EnvironmentEx.GetEnvironmentVariableOrThrow<int>("QUEUE_SHIPPED_PICK_INTERVAL_SECONDS"),
				CheckQueueEverySeconds = EnvironmentEx.GetEnvironmentVariableOrThrow<int>("QUEUE_SHIPPED_INTERVAL_SECONDS")
			}
				};

				var tasks = new List<Task>();

				Console.WriteLine($"[QueueMessageProcessor] Task staggering delay is {EnvironmentEx.GetEnvironmentVariableOrThrow<int>("STAGGER_TASKS_SECONDS")} seconds");

				foreach (var processor in processors)
				{
					Console.WriteLine($"[QueueMessageProcessor] Starting {processor.QueueName} processor...");
					tasks.Add(ConsumeQueuePeriodically(token, processor));


					// stagger startup
					await Task.Delay(TimeSpan.FromSeconds(EnvironmentEx.GetEnvironmentVariableOrThrow<int>("STAGGER_TASKS_SECONDS")), token);
				}

				Console.WriteLine("[QueueMessageProcessor] All processor tasks started.");
				await Task.WhenAll(tasks);
			}
			catch (OperationCanceledException)
			{
				Console.WriteLine("[QueueMessageProcessor] Shutdown requested.");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"[QueueMessageProcessor] Exception: {ex}");
				cts.Cancel();
				throw;
			}
		}

		private async Task ConsumeQueuePeriodically(CancellationToken cancellationToken, QueueMessageProcessorBase processor)
		{
			var queueCheckIntervalSeconds = processor.CheckQueueEverySeconds;
			do
			{
				try
				{
					processor.BeginProcessingMessages();
					if (processor.CheckForMessages())
					{
						processor.ProcessMessages();
					}
					processor.EndProcessingMessages();
					if (!cancellationToken.IsCancellationRequested)
					{
						Console.WriteLine($"[Processor for {processor.QueueName}] Waiting {queueCheckIntervalSeconds} seconds before checking queue again.");
						await Task.Delay(queueCheckIntervalSeconds * 1000);
					}
				}
				catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
				{
					Console.WriteLine($"[{processor.QueueName}] task cancelled.");
					break;
				}
				catch (Exception ex)
				{
					Console.WriteLine($"[{processor.QueueName}] Exception {ex.Message}.");
				}
			} while (true);

			Console.WriteLine($"[{processor.QueueName}] Task exiting.");
		}
	}
}
