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
				RabbitMQUtils.EnsureQueueExists(connection, channel, QueueConstants.ORDERS_APPROVED_QUEUE);
				RabbitMQUtils.EnsureQueueExists(connection, channel, QueueConstants.ORDERS_PENDING_QUEUE);
				RabbitMQUtils.EnsureQueueExists(connection, channel, QueueConstants.ORDERS_READYFORPICKUP_QUEUE);
				RabbitMQUtils.EnsureQueueExists(connection, channel, QueueConstants.ORDERS_CANCELLED_QUEUE);
				RabbitMQUtils.EnsureQueueExists(connection, channel, QueueConstants.ORDERS_COMPLETED_QUEUE);
				RabbitMQUtils.EnsureQueueExists(connection, channel, QueueConstants.ORDERS_SHIPPED_QUEUE);
			}
		}

		[HttpPost]
		[Authorize]
		[Route("OrderApproved")]
		public ResponseDto OrderApproved(OrderHeaderDto orderHeader)
		{
			RabbitMQUtils.PostMessage(_connection.CreateModel(), QueueConstants.ORDERS_APPROVED_QUEUE, System.Text.Json.JsonSerializer.Serialize(orderHeader));
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
			RabbitMQUtils.PostMessage(_connection.CreateModel(), QueueConstants.ORDERS_PENDING_QUEUE, System.Text.Json.JsonSerializer.Serialize(orderHeader));
			return new ResponseDto
			{
				IsSuccess = true,
				Result = orderHeader,
			};
		}

		[HttpPost]
		[Authorize]
		[Route("OrderReadyForPickup")]
		public ResponseDto OrderReadyForPickup(OrderHeaderDto orderHeader)
		{
			RabbitMQUtils.PostMessage(_connection.CreateModel(), QueueConstants.ORDERS_READYFORPICKUP_QUEUE, System.Text.Json.JsonSerializer.Serialize(orderHeader));
			return new ResponseDto
			{
				IsSuccess = true,
				Result = orderHeader,
			};
		}

		[HttpPost]
		[Authorize]
		[Route("OrderCancelled")]
		public ResponseDto OrderCancelled(OrderHeaderDto orderHeader)
		{
			RabbitMQUtils.PostMessage(_connection.CreateModel(), QueueConstants.ORDERS_CANCELLED_QUEUE, System.Text.Json.JsonSerializer.Serialize(orderHeader));
			return new ResponseDto
			{
				IsSuccess = true,
				Result = orderHeader,
			};
		}

		[HttpPost]
		[Authorize]
		[Route("OrderCompleted")]
		public ResponseDto OrderCompleted(OrderHeaderDto orderHeader)
		{
			RabbitMQUtils.PostMessage(_connection.CreateModel(), QueueConstants.ORDERS_COMPLETED_QUEUE, System.Text.Json.JsonSerializer.Serialize(orderHeader));
			return new ResponseDto
			{
				IsSuccess = true,
				Result = orderHeader,
			};
		}

		[HttpPost]
		[Authorize]
		[Route("OrderShipped")]
		public ResponseDto OrderShipped(OrderHeaderDto orderHeader)
		{
			RabbitMQUtils.PostMessage(_connection.CreateModel(), QueueConstants.ORDERS_SHIPPED_QUEUE, System.Text.Json.JsonSerializer.Serialize(orderHeader));
			return new ResponseDto
			{
				IsSuccess = true,
				Result = orderHeader,
			};
		}

	}
}
