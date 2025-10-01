using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using System.Text;

namespace Mango.MessageBus
{
    public interface IMessageBus
    {
        Task PublishMessage(object message, string topic_queue_Name);
    }
}
