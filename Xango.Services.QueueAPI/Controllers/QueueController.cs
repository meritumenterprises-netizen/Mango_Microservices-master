using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using Xango.Models.Dto;
using Xango.Services.Dto;
using Xango.Services.Interfaces;
using Xango.Services.RabbitMQ.Utility;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using IConnection = RabbitMQ.Client.IConnection;
using IModel = RabbitMQ.Client.IModel;


namespace Xango.Services.Queue.Controllers
{
	[ApiController]
	[Route("api/queue")]
	public class QueueController : ControllerBase, IDisposable
	{
		private IConnection _connection;
		private RabbitMQUtils _rabbitMqUtils;
		public QueueController(IConnection connection)
		{
			this._connection = connection;
			this._rabbitMqUtils = new RabbitMQUtils();
		}

		[HttpPost]
		[Authorize]
		[Route("OrderApproved")]
		public ResponseDto OrderApproved(OrderHeaderDto orderHeader)
		{
			this._rabbitMqUtils.EnsureQueueExists(_connection, QueueConstants.ORDERS_APPROVED_QUEUE);
			using (var channel = _connection.CreateModel())
			{
				_rabbitMqUtils.PostMessage(channel, QueueConstants.ORDERS_APPROVED_QUEUE, System.Text.Json.JsonSerializer.Serialize(orderHeader));
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
			this._rabbitMqUtils.EnsureQueueExists(_connection, QueueConstants.ORDERS_PENDING_QUEUE);
			using (var channel = _connection.CreateModel())
			{
				_rabbitMqUtils.PostMessage(channel, QueueConstants.ORDERS_PENDING_QUEUE, System.Text.Json.JsonSerializer.Serialize(orderHeader));
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
			_rabbitMqUtils.EnsureQueueExists(_connection, QueueConstants.ORDERS_READYFORPICKUP_QUEUE);
			using (var channel = _connection.CreateModel())
			{
				_rabbitMqUtils.PostMessage(channel, QueueConstants.ORDERS_READYFORPICKUP_QUEUE, System.Text.Json.JsonSerializer.Serialize(orderHeader));
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
			_rabbitMqUtils.EnsureQueueExists(_connection, QueueConstants.ORDERS_CANCELLED_QUEUE);
			using (var channel = _connection.CreateModel())
			{
				_rabbitMqUtils.PostMessage(channel, QueueConstants.ORDERS_CANCELLED_QUEUE, System.Text.Json.JsonSerializer.Serialize(orderHeader));
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
			_rabbitMqUtils.EnsureQueueExists(_connection, QueueConstants.ORDERS_COMPLETED_QUEUE);
			using (var channel = _connection.CreateModel())
			{
				_rabbitMqUtils.PostMessage(channel, QueueConstants.ORDERS_COMPLETED_QUEUE, System.Text.Json.JsonSerializer.Serialize(orderHeader));
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
			_rabbitMqUtils.EnsureQueueExists(_connection, QueueConstants.ORDERS_SHIPPED_QUEUE);
			using (var channel = _connection.CreateModel())
			{
				_rabbitMqUtils.PostMessage(channel, QueueConstants.ORDERS_SHIPPED_QUEUE, System.Text.Json.JsonSerializer.Serialize(orderHeader));
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
