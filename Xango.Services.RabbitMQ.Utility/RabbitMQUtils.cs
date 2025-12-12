using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace Xango.Services.RabbitMQ.Utility
{
	public class RabbitMQUtils
	{

		public static void EnsureQueueExists(IConnection connection, IModel channel, string queueName)
		{
			try
			{
				// 1. PASSIVE: only checks existence; DOES NOT create the queue
				channel.QueueDeclarePassive(queueName);
				Console.WriteLine($"Queue '{queueName}' already exists.");
			}
			catch (OperationInterruptedException ex)
			{
				// 2. 404 means queue does not exist
				if (ex.ShutdownReason != null && ex.ShutdownReason.ReplyCode == 404)
				{
					Console.WriteLine($"Queue '{queueName}' does not exist. Creating...");

					using (var createChannel = connection.CreateModel())
					{
						createChannel.QueueDeclare(queue: queueName,
												   durable: true,
												   exclusive: false,
												   autoDelete: false,
												   arguments: null);
						Console.WriteLine($"Queue '{queueName}' created.");
					}
				}
				else
				{
					// Any other error rethrow
					throw;
				}
			}
			catch (Exception ex)
			{

			}
		}

		public static void PostMessage(IModel channel, string queueName, string message)
		{
			try
			{
				var body = System.Text.Encoding.UTF8.GetBytes(message);
				channel.BasicPublish(exchange: "",
									 routingKey: queueName,
									 basicProperties: null,
									 body: body);
				Console.WriteLine($" [x] Sent {message}");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error publishing message to queue '{queueName}': {ex.Message}");
			}
		}
	}
}
