
using Xango.Models.Dto;

namespace Xango.Services.Interfaces
{
    public interface IInventoryService
    {
        Task<ResponseDto?> SetProductInStock(int productId, int quantity);
        Task<ResponseDto?> CurrentStock(int productId);
        Task<ResponseDto?> IsProductInStock(int productId);
        Task<ResponseDto?> SubtractFromStock(int productId, int quantity);
    }
}
