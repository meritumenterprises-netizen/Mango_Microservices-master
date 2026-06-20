using Xango.Models.Dto;

namespace Xango.Services.Queue.Services;

public interface IQueueCleanupService
{
	Task<ResponseDto> DeleteOrderFromQueue(string queueName, int orderHeaderId);
}
