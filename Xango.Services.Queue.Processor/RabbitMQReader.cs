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

		public async void Start()
		{
			var cts = new CancellationTokenSource();
			var token = cts.Token;
			try
			{
				var ordersPendingProcessor = new OrdersPendingProcessor(_serviceProvider, cts) { PickMessageOlderThanSeconds = 10 };
				var ordersCancelledProcessor = new OrdersCancelledProcessor(_serviceProvider, cts) { PickMessageOlderThanSeconds = 10 };
				var ordersReadyForPickupProcessor = new OrdersReadyForPickupProcessor(_serviceProvider, cts) { PickMessageOlderThanSeconds = 10 };
				var ordersApprovedProcessor = new OrdersApprovedProcessor(_serviceProvider, cts) { PickMessageOlderThanSeconds = 10 };
				var tasks = new List<Task> // if one task cancels, all should cancel
				{
					ConsumeQueuePeriodically(token, ordersCancelledProcessor),
					ConsumeQueuePeriodically(token, ordersReadyForPickupProcessor),
					ConsumeQueuePeriodically(token, ordersApprovedProcessor),
					ConsumeQueuePeriodically(token, ordersPendingProcessor)
				};
				await Task.WhenAll(tasks);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"[QueueMessageProcessor] Exception: {ex.Message}");
			}
		}


		private async Task ConsumeQueuePeriodically(CancellationToken cancellationToken, QueueMessageProcessorBase processor)
		{
			var queueCheckIntervalSeconds = Convert.ToInt32(Environment.GetEnvironmentVariable("QUEUE_CHECK_INTERVAL_SECONDS"));
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
