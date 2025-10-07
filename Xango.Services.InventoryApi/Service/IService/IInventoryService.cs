using Xango.Models.Dto;

namespace Xango.Services.InventoryApi.Service.IService
{
    public interface IInventoryService
    {
        Task<ProductDto> SetProductInStock(int productId, int quantity);
        Task<int> CurrentStock(int productId);
        Task<bool> IsProductInStock(int productId);
        Task<ProductDto> SubtractFromStock(int productId, int quantity);
        Task<ProductDto> ReturnQty(int productId, int quantity);
    }
}
