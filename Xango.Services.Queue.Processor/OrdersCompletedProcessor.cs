using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xango.Models.Dto;
using Xango.Services.Client.Utility;
using Xango.Services.Server.Utility;
using static Xango.Services.Queue.Processor.QueueMessage;

namespace Xango.Services.Queue.Processor
{
	internal class OrdersCompletedProcessor : QueueMessageProcessorBase
	{
		internal OrdersCompletedProcessor(IServiceProvider _serviceProvider, CancellationTokenSource cancellationTokenSource) :
			base(QueueConstants.ORDERS_COMPLETED_QUEUE(), _serviceProvider, cancellationTokenSource)
		{
			PickMessageOlderThanSeconds = EnvironmentEx.GetEnvironmentVariableOrThrow<int>("QUEUE_COMPLETED_PICK_INTERVAL_SECONDS");
			CheckQueueEverySeconds = EnvironmentEx.GetEnvironmentVariableOrThrow<int>("QUEUE_COMPLETED_INTERVAL_SECONDS");
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
					if (correspondingOrderHeader == null || correspondingOrderHeader.Status != SD.Status_Completed)
					{
						Console.WriteLine($"[{this.GetType().FullName}] Unable to retrieve order with ID {orderHeader.OrderHeaderId} and status Completed.");
						return true;
					}

					var response = this.OrderClient.UpdateOrderStatus(orderHeader.OrderHeaderId, SD.Status_Shipped).Result;
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
