using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using Xango.Models.Dto;
using Xango.Services.Client.Utility;
using Xango.Services.RabbitMQ.Utility;
using static Xango.Services.Queue.Processor.QueueMessage;


namespace Xango.Services.Queue.Processor
{
	internal class OrdersPendingProcessor : QueueMessageProcessorBase
	{
		internal OrdersPendingProcessor(IServiceProvider _serviceProvider, CancellationTokenSource cancellationTokenSource) : 
			base(QueueConstants.ORDERS_PENDING_QUEUE, _serviceProvider, cancellationTokenSource)
		{
		}
		protected override bool ProcessSingleMessage(QueueMessage message)
		{
			bool processed = false;

			if (this.AuthClient != null && this.AuthToken != null && this.OrderClient != null)
			{
				var orderHeader = message.OrderHeader;
				Console.WriteLine($"[{this.GetType().FullName}] Processing order with ID {orderHeader.OrderHeaderId}.");
				try
				{
					var correspondingOrderHeader = DtoConverter.ToDto<OrderHeaderDto>(this.OrderClient.GetOrder(orderHeader.OrderHeaderId).Result);
					if (correspondingOrderHeader == null || correspondingOrderHeader.Status != SD.Status_Pending)
					{
						Console.WriteLine($"[{this.GetType().FullName}] Unable to retrieve order with ID {orderHeader.OrderHeaderId} and status Pending.");
						return true;
					}

					var response = this.OrderClient.UpdateOrderStatus(orderHeader.OrderHeaderId, SD.Status_Cancelled).Result;
					if (response != null && response.IsSuccess)
					{
						processed = true;
					}
				}
				catch (Exception exc)
				{
					Console.WriteLine($"[{this.GetType().FullName}] Exception: {exc.Message}");
				}
			}

			if (processed)
			{
				Console.WriteLine($"[{this.GetType().FullName}] Successfully processed order with ID {message.OrderHeader.OrderHeaderId}.");
			}
			else
			{
				Console.WriteLine($"[{this.GetType().FullName}] Failed to process order with ID {message.OrderHeader.OrderHeaderId}.");
			}
			return processed;
		}
	}
}
