using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xango.Services.Client.Utility;

namespace Xango.Services.Server.Utility
{
	public class QueueConstants
	{
		public static string RABBITMQ_HOST() => EnvironmentEx.GetEnvironmentVariableOrThrow<string>("RABBITMQ_HOST");
		public static string RABBITMQ_USER() => EnvironmentEx.GetEnvironmentVariableOrThrow<string>("RABBITMQ_USER");
		public static string RABBITMQ_PASSWORD() => EnvironmentEx.GetEnvironmentVariableOrThrow<string>("RABBITMQ_PASSWORD");

		public static string ORDERS_CANCELLED_QUEUE() => EnvironmentEx.GetEnvironmentVariableOrThrow<string>("ORDERS_CANCELLED_QUEUE");
		public static string ORDERS_APPROVED_QUEUE() => EnvironmentEx.GetEnvironmentVariableOrThrow<string>("ORDERS_APPROVED_QUEUE");
		public static string ORDERS_PENDING_QUEUE() => EnvironmentEx.GetEnvironmentVariableOrThrow<string>("ORDERS_PENDING_QUEUE");
		public static string ORDERS_READYFORPICKUP_QUEUE() => EnvironmentEx.GetEnvironmentVariableOrThrow<string>("ORDERS_READYFORPICKUP_QUEUE");
		public static string ORDERS_COMPLETED_QUEUE() => EnvironmentEx.GetEnvironmentVariableOrThrow<string>("ORDERS_COMPLETED_QUEUE");
		public static string ORDERS_SHIPPED_QUEUE() => EnvironmentEx.GetEnvironmentVariableOrThrow<string>("ORDERS_SHIPPED_QUEUE");
	}
}
