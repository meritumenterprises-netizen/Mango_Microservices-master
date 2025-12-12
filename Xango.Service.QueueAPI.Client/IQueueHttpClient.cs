using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xango.Models.Dto;
using Xango.Services.Dto;
using Xango.Services.Interfaces;

namespace Xango.Service.QueueAPI.Client
{
	public interface IQueueHttpClient : IQueueService, ISetToken
	{

	}
}
