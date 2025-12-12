using Microsoft.AspNetCore.Mvc;
using Xango.Services.Interfaces;
using Xango.Services.Dto;
using Xango.Models.Dto;
using Microsoft.AspNetCore.Authorization;

namespace Xango.Services.Queue.Controllers
{
	[ApiController]
	[Route("api/queue")]
	public class QueueController : ControllerBase
	{
		public QueueController()
		{
		}

		[HttpPost]
		[Authorize]
		[Route("OrderApproved")]
		public ResponseDto PostOrderApproved(OrderHeaderDto orderHeader)
		{
			return new ResponseDto
			{
				IsSuccess = true,
				Result = orderHeader,
			};
		}

		[HttpPost]
		[Authorize]
		[Route("OrderPending")]
		public ResponseDto PostOrderPending(OrderHeaderDto orderHeader)
		{
			return new ResponseDto
			{
				IsSuccess = true,
				Result = orderHeader,
			};
		}
	}
}
