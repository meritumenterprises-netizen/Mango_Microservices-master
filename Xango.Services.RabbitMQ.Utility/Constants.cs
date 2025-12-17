using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xango.Services.RabbitMQ.Utility
{
	public static class QueueConstants
	{
		public const string ORDERS_CANCELLED_QUEUE = "Xango.Orders.Cancelled";
		public const string ORDERS_APPROVED_QUEUE = "Xango.Orders.Approved";
		public const string ORDERS_PENDING_QUEUE = "Xango.Orders.Pending";
		public const string ORDERS_READYFORPICKUP_QUEUE = "Xango.Orders.ReadyForPickup";
		public const string ORDERS_COMPLETED_QUEUE = "Xango.Orders.Completed";
		public const string ORDERS_SHIPPED_QUEUE = "Xango.Orders.Shipped";
	}
}
