using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using IConnection = RabbitMQ.Client.IConnection;	

namespace Xango.Service.RabbitMQPublisher
{
	public class RabbitMqPublisher : IRabbitMqPublisher, IDisposable
	{
		private readonly IConnection _connection;
		private readonly IModel _channel;

		public RabbitMqPublisher(IConnection connection)
		{
			_connection = connection ?? throw new ArgumentNullException(nameof(connection));
			_channel = _connection.CreateModel();
		}

		public void Publish<T>(string queueName, T message)
		{
			// Declare queue once (idempotent)
			this.EnsureQueueExists(queueName);

			var body = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(message);
			_channel.BasicPublish(exchange: "",
								  routingKey: queueName,
								  basicProperties: null,
								  body: body);
		}

		public void EnsureQueueExists(string queueName)
		{
			_channel.QueueDeclare(queue: queueName,
								  durable: true,
								  exclusive: false,
								  autoDelete: false,
								  arguments: null);
		}

		public void Dispose()
		{
			_channel?.Close();
			_channel?.Dispose();
			// Do NOT dispose connection here if injected as singleton
		}
	}
}

