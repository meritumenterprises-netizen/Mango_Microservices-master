using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using Xango.Models.Dto;
using Xango.Services.Dto;
using Xango.Services.Interfaces;
using Xango.Services.RabbitMQ.Utility;
using IModel = RabbitMQ.Client.IModel;
using IConnection = RabbitMQ.Client.IConnection;


namespace Xango.Services.Queue.Controllers
{
	[ApiController]
	[Route("api/queue")]
	public class QueueController : ControllerBase
	{
		private IConnection _connection;
		public QueueController(IConnection connection)
		{
			this._connection = connection;
			using (var channel = connection.CreateModel())
			{
				RabbitMQUtils.EnsureQueueExists(connection, channel, "Xango.Orders.Approved");
				RabbitMQUtils.EnsureQueueExists(connection, channel, "Xango.Orders.Pending");
				RabbitMQUtils.EnsureQueueExists(connection, channel, "Xango.Orders.ReadyForPickup");
				RabbitMQUtils.EnsureQueueExists(connection, channel, "Xango.Orders.Cancelled");
			}
		}

		[HttpPost]
		[Authorize]
		[Route("OrderApproved")]
		public ResponseDto OrderApproved(OrderHeaderDto orderHeader)
		{
			RabbitMQUtils.PostMessage(_connection.CreateModel(), "Xango.Orders.Approved", System.Text.Json.JsonSerializer.Serialize(orderHeader));
			return new ResponseDto
			{
				IsSuccess = true,
				Result = orderHeader,
			};
		}

		[HttpPost]
		[Authorize]
		[Route("OrderPending")]
		public ResponseDto OrderPending(OrderHeaderDto orderHeader)
		{
			RabbitMQUtils.PostMessage(_connection.CreateModel(), "Xango.Orders.Pending", System.Text.Json.JsonSerializer.Serialize(orderHeader));
			return new ResponseDto
			{
				IsSuccess = true,
				Result = orderHeader,
			};
		}
	}
}
