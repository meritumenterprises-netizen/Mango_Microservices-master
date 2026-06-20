using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MassTransit;
using Xango.Models.Dto;
using Xango.Services.Client.Utility;
using Xango.Services.Queue.Services;
using Xango.Services.Server.Utility;

namespace Xango.Services.Queue.Controllers
{
	[ApiController]
	[Route("api/queue")]
	public class QueueController : ControllerBase
	{
		private readonly ISendEndpointProvider _sendEndpointProvider;
		private readonly IQueueCleanupService _queueCleanupService;

		public QueueController(
			ISendEndpointProvider sendEndpointProvider,
			IQueueCleanupService queueCleanupService)
		{
			_sendEndpointProvider = sendEndpointProvider;
			_queueCleanupService = queueCleanupService;
		}

		[HttpPost]
		[Authorize]
		[Route("OrderApproved")]
		public async Task<ResponseDto> OrderApproved(OrderHeaderDto orderHeader)
		{
			return await SendOrder(QueueConstants.ORDERS_APPROVED_QUEUE(), orderHeader);
		}

		[HttpPost]
		[Authorize]
		[Route("OrderPending")]
		public async Task<ResponseDto> OrderPending(OrderHeaderDto orderHeader)
		{
			return await SendOrder(QueueConstants.ORDERS_PENDING_QUEUE(), orderHeader);
		}

		[HttpPost]
		[Authorize]
		[Route("OrderReadyForPickup")]
		public async Task<ResponseDto> OrderReadyForPickup(OrderHeaderDto orderHeader)
		{
			return await SendOrder(QueueConstants.ORDERS_READYFORPICKUP_QUEUE(), orderHeader);
		}

		[HttpPost]
		[Authorize]
		[Route("OrderCancelled")]
		public async Task<ResponseDto> OrderCancelled(OrderHeaderDto orderHeader)
		{
			return await SendOrder(QueueConstants.ORDERS_CANCELLED_QUEUE(), orderHeader);
		}

		[HttpPost]
		[Authorize]
		[Route("OrderCompleted")]
		public async Task<ResponseDto> OrderCompleted(OrderHeaderDto orderHeader)
		{
			return await SendOrder(QueueConstants.ORDERS_COMPLETED_QUEUE(), orderHeader);
		}

		[HttpPost]
		[Authorize]
		[Route("OrderShipped")]
		public async Task<ResponseDto> OrderShipped(OrderHeaderDto orderHeader)
		{
			return await SendOrder(QueueConstants.ORDERS_SHIPPED_QUEUE(), orderHeader);
		}

		[HttpDelete]
		[Authorize]
		[Route("OrderCompleted/{orderHeaderId:int}")]
		public async Task<ResponseDto> DeleteOrderFromCompleted(int orderHeaderId)
		{
			return await _queueCleanupService.DeleteOrderFromQueue(
				QueueConstants.ORDERS_COMPLETED_QUEUE(),
				orderHeaderId);
		}

		[HttpDelete]
		[Authorize]
		[Route("Order/{orderHeaderId:int}/Status/{status}")]
		public async Task<ResponseDto> DeleteOrderFromStatusQueue(int orderHeaderId, string status)
		{
			var queueName = GetQueueName(status);
			if (queueName == null)
			{
				return new ResponseDto
				{
					IsSuccess = false,
					Message = $"No queue is configured for order status '{status}'."
				};
			}

			return await _queueCleanupService.DeleteOrderFromQueue(queueName, orderHeaderId);
		}

		private async Task<ResponseDto> SendOrder(string queueName, OrderHeaderDto orderHeader)
		{
			orderHeader.ModifiedTime = DateTime.Now;
			var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{queueName}"));
			await endpoint.Send(orderHeader);

			return new ResponseDto
			{
				IsSuccess = true,
				Result = orderHeader,
			};
		}

		private static string? GetQueueName(string status)
		{
			return NormalizeOrderStatus(status) switch
			{
				SD.Status_Pending => QueueConstants.ORDERS_PENDING_QUEUE(),
				SD.Status_Approved => QueueConstants.ORDERS_APPROVED_QUEUE(),
				SD.Status_ReadyForPickup => QueueConstants.ORDERS_READYFORPICKUP_QUEUE(),
				SD.Status_Completed => QueueConstants.ORDERS_COMPLETED_QUEUE(),
				SD.Status_Cancelled => QueueConstants.ORDERS_CANCELLED_QUEUE(),
				SD.Status_Shipped => QueueConstants.ORDERS_SHIPPED_QUEUE(),
				_ => null
			};
		}

		private static string NormalizeOrderStatus(string status)
		{
			var compactStatus = status.Trim().Replace(" ", "", StringComparison.Ordinal);
			return compactStatus.ToLowerInvariant() switch
			{
				"pending" => SD.Status_Pending,
				"approved" => SD.Status_Approved,
				"readyforpickup" => SD.Status_ReadyForPickup,
				"completed" => SD.Status_Completed,
				"cancelled" => SD.Status_Cancelled,
				"canceled" => SD.Status_Cancelled,
				"shipped" => SD.Status_Shipped,
				_ => status.Trim()
			};
		}
	}
}
