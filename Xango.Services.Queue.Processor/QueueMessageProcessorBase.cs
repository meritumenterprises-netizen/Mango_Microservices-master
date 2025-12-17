using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using Xango.Models.Dto;
using Xango.Services.Client.Utility;
using Xango.Service.AuthenticationAPI.Client;
using Xango.Service.OrderAPI.Client;
using IConnection = RabbitMQ.Client.IConnection;
using Microsoft.Extensions.DependencyInjection;


namespace Xango.Services.Queue.Processor
{
	public class QueueMessage
	{
		public OrderHeaderDto OrderHeader { get; set; }
	}

	public abstract class QueueMessageProcessorBase
	{
		protected IConnection RabbitMqConnection { get; set; }
		protected IAuthenticationHttpClient AuthClient { get; set; }
		protected IOrderHttpClient OrderClient { get; set; }
		protected string AuthToken { get; set; }
		public CancellationToken CancellationToken { get; private set; }
		protected CancellationTokenSource CancellationTokenSource { get; set; }
		protected bool StartedProcessingMessages { get; set; } = false;

		public TimeSpan MaxRunTime { get; set; }
		public DateTime StartTime { get; set; }


		public int CheckQueueEverySeconds { get; set; }
		public int PickMessageOlderThanSeconds { get; set; }
		public string QueueName { get; protected set; }

		protected Queue<QueueMessage> OrderQueue { get; set; }
		protected IServiceProvider ServiceProvider { get; set; }
		protected QueueMessageProcessorBase(string queueName, IServiceProvider serviceProvider, CancellationTokenSource cancellationTokenSource, int pickMessagesOlderThanSeconds = 3600, int checkQueueEverySeconds = 60)
		{
			this.CancellationTokenSource = cancellationTokenSource;
			this.CancellationToken = this.CancellationTokenSource.Token;
			this.CheckQueueEverySeconds = checkQueueEverySeconds;
			this.OrderQueue = new Queue<QueueMessage>();
			this.QueueName = queueName;
			this.PickMessageOlderThanSeconds = pickMessagesOlderThanSeconds;
			this.CheckQueueEverySeconds = checkQueueEverySeconds;
			this.ServiceProvider = serviceProvider;
			this.AuthClient = this.ServiceProvider.GetRequiredService<IAuthenticationHttpClient>();
			this.OrderClient = this.ServiceProvider.GetRequiredService<IOrderHttpClient>();
			this.MaxRunTime = TimeSpan.FromMinutes(EnvironmentEx.GetEnvironmentVariableOrThrow<int>("MAX_RUNTIME_MINUTES"));
			this.StartTime = DateTime.Now;

			Console.WriteLine($"{this.GetType().FullName} Initialized at {this.StartTime.ToString("dd.MM.yyyy HH:mm:ss")}");
			Console.WriteLine($"{this.GetType().FullName} Maximum run time of the processor is {(this.MaxRunTime == TimeSpan.Zero ? "indefinite" : this.MaxRunTime.ToString() + " minutes")}");
		}

		protected void AddOrderMessage(OrderHeaderDto orderHeader)
		{
			Console.WriteLine($"[{this.GetType().FullName}] Adding order id {orderHeader.OrderHeaderId} with status {orderHeader.Status} to the processing queue...");
			this.OrderQueue.Enqueue(new QueueMessage() { OrderHeader = orderHeader });
		}

		internal void Begin()
		{
			Console.WriteLine($"{this.GetType().FullName} Check queue every {this.CheckQueueEverySeconds} seconds");
			Console.WriteLine($"{this.GetType().FullName} Pick messages older than {this.PickMessageOlderThanSeconds} seconds");
			try
			{
				this.RabbitMqConnection = this.ServiceProvider.GetRequiredService<IConnection>();
				Console.WriteLine($"[{this.GetType().FullName}] Created connection to the message queue broker");
				var loginRequest = new LoginRequestDto()
				{
					UserName = Environment.GetEnvironmentVariable("AUTH_USER"),
					Password = Environment.GetEnvironmentVariable("AUTH_PASSWORD")
				};
				var response = this.AuthClient.Login(loginRequest).Result;
				if (response != null && response.IsSuccess)
				{
					var loginResult = DtoConverter.ToDto<LoginResponseDto?>(response);
					this.AuthToken = loginResult.Token;
					this.OrderClient.SetToken(this.AuthToken);
					Console.WriteLine($"[{this.GetType().FullName}] Logged in successfully.");
				}
				else
				{
					this.CancellationTokenSource.Cancel();
					Console.WriteLine($"{this.GetType().FullName}] Failed to authenticate with the Authentication API Client.");
				}
			}
			catch (Exception innerEx) when (innerEx is BrokerUnreachableException || innerEx is OperationInterruptedException)
			{
				Console.WriteLine($"[{this.GetType().FullName}] Unrecoverable exception: " + innerEx.Message);
				throw new FatalMessageBrokerException("Unable to connect to the message broker.", innerEx);
			}

			catch (Exception ex) when (ex is BrokerUnreachableException || ex is OperationInterruptedException)
			{
				Console.WriteLine($"[{this.GetType().FullName}] Exception: " + ex.Message);
				this.CancellationTokenSource.Cancel();
			}
			catch (Exception ex)
			{
				Console.WriteLine($"[{this.GetType().FullName}] Error creating queue connection: " + ex.Message);
			}
		}

		internal void End()
		{
			var response = this.AuthClient?.Logout().Result;
			if (response != null && response.IsSuccess)
			{
				Console.WriteLine($"[{this.GetType().FullName}] Logged out successfully.");
			}

			this.CloseMessageQueueConnection();
		}

		internal void BeginProcessingMessages()
		{
			Console.WriteLine($"[{this.GetType().FullName}] BeginProcessingMessages starting...");
			this.StartedProcessingMessages = true;

			Console.WriteLine($"[{this.GetType().FullName}] BeginProcessMessages ending...");
		}

		internal void EndProcessingMessages()
		{
			Console.WriteLine($"[{this.GetType().FullName}] EndProcessingMessages starting...");
			this.StartedProcessingMessages = false;
			if (this.MaxRunTime.TotalMinutes != 0)
			{
				var runDuration = DateTime.Now - this.StartTime;
				Console.WriteLine($"[{this.GetType().FullName}] Task running duration: {runDuration.TotalMinutes:F2} minutes.");
				if (runDuration >= this.MaxRunTime)
				{
					Console.WriteLine($"[{this.GetType().FullName}] Max run time of {this.MaxRunTime.TotalMinutes} minutes reached. Stopping processor.");
					this.CancellationTokenSource.Cancel();
					//this.CancellationToken.ThrowIfCancellationRequested();
				}
			}
			Console.WriteLine($"[{this.GetType().FullName}] EndMessageProcessing ending...");
		}

		protected bool AreOrderMessagesAvailable()
		{
			Console.WriteLine($"[{this.GetType().FullName}] Messages are {(this.OrderQueue.Count > 0 ? "available" : "unavailable")}");
			return this.OrderQueue.Count > 0;
		}

		public virtual bool CheckForMessages()
		{

			Console.WriteLine($"[{this.GetType().FullName}] Starting checking for messages...");
			BasicGetResult message = null;
			using (var channel = this.RabbitMqConnection.CreateModel())
			{
				do
				{
					Console.WriteLine($"[{this.GetType().FullName}] Message queue {this.QueueName} has {channel.MessageCount(this.QueueName)} available message(s)...");
					if (this.CancellationToken.IsCancellationRequested)
					{
						return false;
					}
					try
					{
						Console.WriteLine($"[{this.GetType().FullName}] Retrieving message from queue {this.QueueName}...");
						message = channel.BasicGet(this.QueueName, autoAck: false);
						if (message != null)
						{
							Console.WriteLine($"[{this.GetType().FullName}] Retrieved message from queue {this.QueueName}...");
							var orderHeader = DtoConverter.ToDto<OrderHeaderDto>(Encoding.UTF8.GetString(message.Body.ToArray()));
							if (orderHeader != null)
							{
								Console.WriteLine($"[{this.GetType().FullName}] Found message for order id {orderHeader.OrderHeaderId}...");
								var messageAgeSeconds = (DateTime.Now - orderHeader.ModifiedTime).TotalSeconds;
								if (messageAgeSeconds >= this.PickMessageOlderThanSeconds)
								{
									Console.WriteLine($"[{this.GetType().FullName}] Message for order id {orderHeader.OrderHeaderId} qualifies for processing...");
									this.AddOrderMessage(orderHeader);
									channel.BasicAck(message.DeliveryTag, false);
									continue;
								}
								else
								{
									Console.WriteLine($"[{this.GetType().FullName}] Message for order id {orderHeader.OrderHeaderId} does not quality for processing, it is too new...");
									// Not old enough, requeue
									channel.BasicNack(message.DeliveryTag, false, true);
									break;
								}
							}
							else
							{
								// Invalid message, discard
								Console.WriteLine($"[{this.GetType().FullName}] Message for order id {orderHeader.OrderHeaderId} is invalid, removing message from queue {this.QueueName}...");
								channel.BasicNack(message.DeliveryTag, false, false);
								break;
							}
						}
					}
					catch (Exception ex)
					{
						Console.WriteLine($"[{this.GetType().FullName}] Exception: " + ex.Message);
					}
				} while (message != null);
			}
			Console.WriteLine($"[{this.GetType().FullName}] Ending checking for messages...");
			return this.AreOrderMessagesAvailable();
		}

		protected abstract bool ProcessSingleMessage(QueueMessage message);

		internal void DeleteOrhpanedMessage(QueueMessage message)
		{
			// TODO: Implement logic to delete orphaned message if needed
		}
		public virtual bool ProcessMessages()
		{
			Console.WriteLine($"[{this.GetType().FullName}] Starting processing messages...");
			while (this.AreOrderMessagesAvailable())
			{
				Console.WriteLine($"[{this.GetType().FullName}] Messages are available...");
				var message = this.OrderQueue.Dequeue();
				var result = this.ProcessSingleMessage(message);
				if (result)
				{
					Console.WriteLine($"[{this.GetType().FullName}] Processed message for order id {message.OrderHeader.OrderHeaderId} successfully...");
				}
				else
				{
					Console.WriteLine($"[{this.GetType().FullName}] Failed to process message for Order ID: {message.OrderHeader.OrderHeaderId}");
				}
			}
			Console.WriteLine($"[{this.GetType().FullName}] Ending processing messages...");
			return true;
		}

		public void CloseMessageQueueConnection()
		{
			Console.WriteLine($"[{this.GetType().FullName}] Closing message queue connection...");
			if (this.RabbitMqConnection != null && this.RabbitMqConnection.IsOpen)
			{
				this.RabbitMqConnection.Close();
				this.RabbitMqConnection.Dispose();
			}
		}

		public void Finish()
		{
			Console.WriteLine($"[{this.GetType().FullName}] Processor finished.");
		}
	}
}
