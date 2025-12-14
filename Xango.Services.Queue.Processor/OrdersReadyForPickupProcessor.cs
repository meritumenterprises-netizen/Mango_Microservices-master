using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using Xango.Services.RabbitMQ.Utility;
using static Xango.Services.Queue.Processor.QueueMessage;

namespace Xango.Services.Queue.Processor
{
	internal class OrdersReadyForPickupProcessor : QueueMessageProcessorBase
	{
		internal OrdersReadyForPickupProcessor(ServiceProvider _serviceProvider, CancellationTokenSource cancellationTokenSource) :
			base(QueueConstants.ORDERS_READYFORPICKUP_QUEUE, _serviceProvider, cancellationTokenSource)
		{
		}
		protected override bool ProcessSingleMessage(QueueMessage message)
		{
			bool processed = false;

			if (this.AuthClient != null && this.AuthToken != null && this.OrderClient != null)
			{
				var orderHeader = message.OrderHeader;
				try
				{
					var response = this.OrderClient.UpdateOrderStatus(orderHeader.OrderHeaderId, "Completed").Result;
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

			return processed;
		}

	}
}
