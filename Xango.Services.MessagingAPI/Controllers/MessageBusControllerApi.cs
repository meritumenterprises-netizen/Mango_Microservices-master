using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using System.Text;
using Xango.Services.Dto;

namespace Xango.Services.MessagingAPI.Controllers
{
    [ApiController]
    [Route("api/messaging")]
    public class MessageBusControllerApi : Controller
    {
        private string connectionString = "amqp://guest:guest@localhost:5672/";

        private ResponseDto _response;

        public MessageBusControllerApi()
        {
            _response = new ResponseDto();
        }

        [HttpPost]
        public async Task<ResponseDto> PublishMessage(object messageObj, string topic_queue_Name)
        {
            //var factory = new ConnectionFactory()
            //{
            //    HostName = "localhost", // local machine
            //    UserName = "guest",
            //    Password = "guest"
            //};

            //// Connect to RabbitMQ
            //using (var connection = await factory.CreateConnectionAsync())
            //using (var channel = await connection.CreateChannelAsync())
            //{
            //    // Declare a queue named "log_queue"
            //    await channel.QueueDeclareAsync(
            //        queue: topic_queue_Name,
            //        durable: true,      // queue survives broker restart
            //        exclusive: false,   // can be accessed by other connections
            //        autoDelete: false,
            //        arguments: null
            //    );

            //    string message = Convert.ToString(messageObj);
            //    var body = Encoding.UTF8.GetBytes(message);
            //    // Publish message to the queue
            //    await channel.BasicPublishAsync(
            //        exchange: "",
            //        routingKey: topic_queue_Name,
            //        body: body
            //    );
            //}
            return _response;
        }
    }
}
