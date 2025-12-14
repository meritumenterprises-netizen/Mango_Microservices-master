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
		public BasicGetResult Message { get; set; }
		public OrderHeaderDto OrderHeader { get; set; }
		public bool ProcessedSuccessfully { get; set; } = false;

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

			protected IModel Channel { get; set; }
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
				this.RabbitMqConnection = this.ServiceProvider.GetRequiredService<IConnection>();
				this.AuthClient = this.ServiceProvider.GetRequiredService<IAuthenticationHttpClient>();
				this.OrderClient = this.ServiceProvider.GetRequiredService<IOrderHttpClient>();
				this.MaxRunTime = TimeSpan.FromMinutes(Convert.ToInt32(Environment.GetEnvironmentVariable("MAX_RUNTIME_MINUTES")));
				this.StartTime = DateTime.Now;
			}

			protected void AddOrderMessage(BasicGetResult message, OrderHeaderDto orderHeader)
			{
				this.OrderQueue.Enqueue(new QueueMessage() { Message = message, OrderHeader = orderHeader });
			}

			protected void RemoveOrderMessage()
			{
				this.Channel.BasicAck(this.OrderQueue.Dequeue().Message.DeliveryTag, false);
			}

			internal void BeginProcessingMessages()
			{
				this.Channel = this.RabbitMqConnection.CreateModel();
				this.StartedProcessingMessages = true;

				var loginRequest = new LoginRequestDto() { 
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

			internal void EndProcessingMessages()
			{
				this.Channel.Close();
				this.Channel.Dispose();
				this.StartedProcessingMessages = false;
				var response = this.AuthClient?.Logout().Result;
				if (response != null && response.IsSuccess)
				{
					Console.WriteLine($"[{this.GetType().FullName}] Logged out successfully.");
				}

				if (this.MaxRunTime.TotalMinutes != 0)
				{
					var runDuration = DateTime.Now - this.StartTime;
					Console.WriteLine($"[{this.GetType().FullName}] Run duration: {runDuration.TotalMinutes} minutes.");
					if (runDuration >= this.MaxRunTime)
					{
						Console.WriteLine($"[{this.GetType().FullName}] Max run time of {this.MaxRunTime.TotalMinutes} minutes reached. Stopping processor.");
						this.CancellationTokenSource.Cancel();
						this.CancellationToken.ThrowIfCancellationRequested();
					}
				}
			}

			protected bool AreOrderMessagesAvailable()
			{
				return this.OrderQueue.Count > 0;
			}

			public virtual bool CheckForMessages()
			{
				BasicGetResult message = null;
				do
				{
					if (this.CancellationToken.IsCancellationRequested)
					{
						return false;
					}
					try
					{
						message = this.Channel.BasicGet(this.QueueName, autoAck: false);
						if (message != null)
						{
							var orderHeader = DtoConverter.ToDto<OrderHeaderDto>(Encoding.UTF8.GetString(message.Body.ToArray()));
							if (orderHeader != null)
							{
								var messageAgeSeconds = (DateTime.UtcNow - orderHeader.OrderTime).TotalSeconds;
								if (messageAgeSeconds >= this.PickMessageOlderThanSeconds)
								{
									this.AddOrderMessage(message, orderHeader);
									continue;
								}
								else
								{
									// Not old enough, requeue
									this.Channel.BasicNack(message.DeliveryTag, false, true);
									break;
								}
							}
							else
							{
								// Invalid message, discard
								this.Channel.BasicNack(message.DeliveryTag, false, false);
								message = null;
								break;
							}
						}
					}
					catch (Exception ex) when (ex is BrokerUnreachableException || ex is OperationInterruptedException)
					{
						Console.WriteLine($"[{this.GetType().FullName}] Exception: " + ex.Message);
					}
				} while (message != null);

				return this.AreOrderMessagesAvailable();
			}

			protected abstract bool ProcessSingleMessage(QueueMessage message);
			public virtual bool ProcessMessages()
			{
				while (this.AreOrderMessagesAvailable())
				{
					var message = this.OrderQueue.Peek();
					var result = this.ProcessSingleMessage(message);
					if (result)
					{
						this.RemoveOrderMessage();
					}
					else
					{
						Console.WriteLine($"[{this.GetType().FullName}] Failed to process message for Order ID: {message.OrderHeader.OrderHeaderId}");
						this.RemoveOrderMessage(); // TODO: in the future, replace this with a dead-letter queue mechanism
					}
				}
				return true;
			}

		}	
	}
}
