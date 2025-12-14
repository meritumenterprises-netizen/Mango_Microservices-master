using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xango.Services.Queue.Processor
{
	internal class FatalMessageBrokerException : ApplicationException
	{
		public FatalMessageBrokerException()
		{
		}
		public FatalMessageBrokerException(string message)
			: base(message)
		{
		}
		public FatalMessageBrokerException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}
}
