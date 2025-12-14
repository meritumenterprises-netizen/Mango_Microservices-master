using Xango.Models.Dto;
using Xango.Services.Dto;

namespace Xango.Services.Interfaces
{
	public interface IQueueService
	{
		Task<ResponseDto> PostOrderApproved(OrderHeaderDto orderHeaderDto);
		Task<ResponseDto> PostOrderPending(OrderHeaderDto orderHeaderDto);
		Task<ResponseDto> PostOrderReadyForPickup(OrderHeaderDto orderHeaderDto);
		Task<ResponseDto> PostOrderCancelled(OrderHeaderDto orderHeaderDto);
	}
}
