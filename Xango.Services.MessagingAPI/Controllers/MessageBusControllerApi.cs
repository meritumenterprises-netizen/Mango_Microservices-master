using AutoMapper;
using Confluent.Kafka;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Web;
using Xango.Models.Dto;

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
        [ActionName("PublishMessage")]
        public async Task<ResponseDto> PublishMessage(object messageObj, string topic_queue_Name)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = "localhost:9092"
            };

            using var producer = new ProducerBuilder<string, string>(config).Build();

            //string topic = "Xango.Messaging.EmailOrder";
            string topic = topic_queue_Name;
            string key = "Email";
            //string value = "Hello, Kafka from C#!";

            producer.Produce("Xango.Messaging.Email", new Message<string, string> { Key = key, Value = messageObj.ToString() },
                (deliveryReport) =>
                {
                    if (deliveryReport.Error.IsError)
                    {
                        Console.WriteLine($"Delivery Error: {deliveryReport.Error.Reason}");
                    }
                    else
                    {
                        Console.WriteLine($"Delivered message to {deliveryReport.TopicPartitionOffset}");
                    }
                });

            _response.IsSuccess = true;
            return _response;
        }
    }
}
