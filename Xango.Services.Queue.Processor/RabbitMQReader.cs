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
			Console.WriteLine("[QueueMessageProcessor] Starting processor...");

			var cts = new CancellationTokenSource();
			var token = cts.Token;

			try
			{
				var processors = new QueueMessageProcessorBase[]
				{
					new OrdersCancelledProcessor(_serviceProvider, cts),
					new OrdersReadyForPickupProcessor(_serviceProvider, cts),
					new OrdersApprovedProcessor(_serviceProvider, cts),
					new OrdersPendingProcessor(_serviceProvider, cts),
					new OrdersCompletedProcessor(_serviceProvider, cts),
					new OrdersShippedProcessor(_serviceProvider, cts)
				};

				var tasks = new List<Task>();

				Console.WriteLine($"[{this.GetType().FullName}] Task staggering delay is {EnvironmentEx.GetEnvironmentVariableOrThrow<int>("STAGGER_TASKS_SECONDS")} seconds");

				foreach (var processor in processors)
				{
					Console.WriteLine($"[{this.GetType().FullName}] Starting {processor.QueueName} processor...");
					tasks.Add(ConsumeQueuePeriodically(token, processor));


					// stagger startup
					Console.WriteLine($"[{this.GetType().FullName}] Waiting {EnvironmentEx.GetEnvironmentVariableOrThrow<int>("STAGGER_TASKS_SECONDS")} seconds before starting next processor...");
					await Task.Delay(TimeSpan.FromSeconds(EnvironmentEx.GetEnvironmentVariableOrThrow<int>("STAGGER_TASKS_SECONDS")), token);
				}

				Console.WriteLine($"[{this.GetType().FullName}] All processor tasks are now started.");
				await Task.WhenAll(tasks);
			}
			catch (OperationCanceledException)
			{
				Console.WriteLine($"[{this.GetType().FullName}] Shutdown requested.");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"[{this.GetType().FullName}] Exception: {ex}, cancelling all queue processors");
				cts.Cancel();
				throw;
			}
		}

		private async Task ConsumeQueuePeriodically(CancellationToken cancellationToken, QueueMessageProcessorBase processor)
		{
			var queueCheckIntervalSeconds = processor.CheckQueueEverySeconds;
			processor.Begin();
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
					processor.Finish();
					if (!cancellationToken.IsCancellationRequested)
					{
						Console.WriteLine($"[{this.GetType().FullName}] Waiting {queueCheckIntervalSeconds} seconds before checking queue {processor.QueueName} again.");
						await Task.Delay(queueCheckIntervalSeconds * 1000);
					}
				}
				catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
				{
					Console.WriteLine($"[{processor.GetType().FullName}] task cancelled.");
					break;
				}
				catch (FatalMessageBrokerException ex)
				{
					Console.WriteLine($"[{processor.GetType().FullName}] FatalMessageBrokerException: {ex.Message}, stopping processor.");
					break;
				}
				catch (Exception ex)
				{
					Console.WriteLine($"[{processor.GetType().FullName}] Exception {ex.Message}.");
				}
			} while (true);
			processor.End();

			Console.WriteLine($"[{processor.GetType().FullName}] Processor exiting at {DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")}.");
		}
	}
}
