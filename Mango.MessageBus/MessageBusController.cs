using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using System.Text;

namespace Mango.MessageBus
{
    [ApiController]
    [Route("[controller]")]

    public class MessageBusController : IMessageBus
    {
        private string connectionString = "amqp://guest:guest@localhost:5672/";

        public async Task PublishMessage(object messageObj, string topic_queue_Name)
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost", // local machine
                UserName = "guest",
                Password = "guest"
            };

            // Connect to RabbitMQ
            using (var connection = await factory.CreateConnectionAsync())
            using (var channel = await connection.CreateChannelAsync())
            {
                // Declare a queue named "log_queue"
                await channel.QueueDeclareAsync(
                    queue: topic_queue_Name,
                    durable: true,      // queue survives broker restart
                    exclusive: false,   // can be accessed by other connections
                    autoDelete: false,
                    arguments: null
                );

                string message = Convert.ToString(messageObj);
                var body = Encoding.UTF8.GetBytes(message);
                // Publish message to the queue
                await channel.BasicPublishAsync(
                    exchange: "",
                    routingKey: topic_queue_Name,
                    body: body
                );
            }
        }
    }

}
