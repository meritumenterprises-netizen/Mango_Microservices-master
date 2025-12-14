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
	public class QueueController : ControllerBase, IDisposable
	{
		private IConnection _connection;
		public QueueController(IConnection connection)
		{
			this._connection = connection;
			RabbitMQUtils.EnsureQueueExists(connection, QueueConstants.ORDERS_APPROVED_QUEUE);
			RabbitMQUtils.EnsureQueueExists(connection, QueueConstants.ORDERS_PENDING_QUEUE);
			RabbitMQUtils.EnsureQueueExists(connection, QueueConstants.ORDERS_READYFORPICKUP_QUEUE);
			RabbitMQUtils.EnsureQueueExists(connection, QueueConstants.ORDERS_CANCELLED_QUEUE);
			RabbitMQUtils.EnsureQueueExists(connection, QueueConstants.ORDERS_COMPLETED_QUEUE);
			RabbitMQUtils.EnsureQueueExists(connection, QueueConstants.ORDERS_SHIPPED_QUEUE);
		}

		[HttpPost]
		[Authorize]
		[Route("OrderApproved")]
		public ResponseDto OrderApproved(OrderHeaderDto orderHeader)
		{
			using (var channel = _connection.CreateModel())
			{
				RabbitMQUtils.PostMessage(channel, QueueConstants.ORDERS_APPROVED_QUEUE, System.Text.Json.JsonSerializer.Serialize(orderHeader));
				return new ResponseDto
				{
					IsSuccess = true,
					Result = orderHeader,
				};
			}
		}

		[HttpPost]
		[Authorize]
		[Route("OrderPending")]
		public ResponseDto OrderPending(OrderHeaderDto orderHeader)
		{
			using (var channel = _connection.CreateModel())
			{
				RabbitMQUtils.PostMessage(channel, QueueConstants.ORDERS_PENDING_QUEUE, System.Text.Json.JsonSerializer.Serialize(orderHeader));
				return new ResponseDto
				{
					IsSuccess = true,
					Result = orderHeader,
				};
			}
		}

		[HttpPost]
		[Authorize]
		[Route("OrderReadyForPickup")]
		public ResponseDto OrderReadyForPickup(OrderHeaderDto orderHeader)
		{
			using (var channel = _connection.CreateModel())
			{
				RabbitMQUtils.PostMessage(channel, QueueConstants.ORDERS_READYFORPICKUP_QUEUE, System.Text.Json.JsonSerializer.Serialize(orderHeader));
				return new ResponseDto
				{
					IsSuccess = true,
					Result = orderHeader,
				};
			}
		}

		[HttpPost]
		[Authorize]
		[Route("OrderCancelled")]
		public ResponseDto OrderCancelled(OrderHeaderDto orderHeader)
		{
			using (var channel = _connection.CreateModel())
			{
				RabbitMQUtils.PostMessage(channel, QueueConstants.ORDERS_CANCELLED_QUEUE, System.Text.Json.JsonSerializer.Serialize(orderHeader));
				return new ResponseDto
				{
					IsSuccess = true,
					Result = orderHeader,
				};
			}
		}

		[HttpPost]
		[Authorize]
		[Route("OrderCompleted")]
		public ResponseDto OrderCompleted(OrderHeaderDto orderHeader)
		{
			using (var channel = _connection.CreateModel())
			{
				RabbitMQUtils.PostMessage(channel, QueueConstants.ORDERS_COMPLETED_QUEUE, System.Text.Json.JsonSerializer.Serialize(orderHeader));
				return new ResponseDto
				{
					IsSuccess = true,
					Result = orderHeader,
				};
			}
		}

		[HttpPost]
		[Authorize]
		[Route("OrderShipped")]
		public ResponseDto OrderShipped(OrderHeaderDto orderHeader)
		{
			using (var channel = _connection.CreateModel())
			{
				RabbitMQUtils.PostMessage(channel, QueueConstants.ORDERS_SHIPPED_QUEUE, System.Text.Json.JsonSerializer.Serialize(orderHeader));
				return new ResponseDto
				{
					IsSuccess = true,
					Result = orderHeader,
				};
			}
		}

		public void Dispose()
		{
			if (_connection != null)
			{
				try
				{
					_connection.Close();
					_connection.Dispose();
				}
				catch (AlreadyClosedException)
				{
					// Connection is already closed, nothing to do
				}
			}
		}
	}
}
