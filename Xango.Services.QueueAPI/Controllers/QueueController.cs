using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using Xango.Models.Dto;
using Xango.Service.RabbitMQPublisher;
using Xango.Services.Client.Utility;
using Xango.Services.Server.Utility;
using Xango.Services.Dto;
using Xango.Services.Interfaces;

namespace Xango.Services.Queue.Controllers
{
	[ApiController]
	[Route("api/queue")]
	public class QueueController : ControllerBase
	{
		private IRabbitMqPublisher _rabbitMqPublisher;
		public QueueController(IRabbitMqPublisher rabbitMqPublisher)
		{
			this._rabbitMqPublisher = rabbitMqPublisher;
		}

		[HttpPost]
		[Authorize]
		[Route("OrderApproved")]
		public ResponseDto OrderApproved(OrderHeaderDto orderHeader)
		{
			_rabbitMqPublisher.Publish<OrderHeaderDto>(QueueConstants.ORDERS_APPROVED_QUEUE(), orderHeader);
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
			_rabbitMqPublisher.Publish<OrderHeaderDto>(QueueConstants.ORDERS_PENDING_QUEUE(), orderHeader);
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
			_rabbitMqPublisher.Publish<OrderHeaderDto>(QueueConstants.ORDERS_READYFORPICKUP_QUEUE(), orderHeader);
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
			_rabbitMqPublisher.Publish<OrderHeaderDto>(QueueConstants.ORDERS_CANCELLED_QUEUE(), orderHeader);
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
			_rabbitMqPublisher.Publish<OrderHeaderDto>(QueueConstants.ORDERS_COMPLETED_QUEUE(), orderHeader); return new ResponseDto
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
			_rabbitMqPublisher.Publish<OrderHeaderDto>(QueueConstants.ORDERS_SHIPPED_QUEUE(), orderHeader); return new ResponseDto
			{
				IsSuccess = true,
				Result = orderHeader,
			};
		}
	}
}
