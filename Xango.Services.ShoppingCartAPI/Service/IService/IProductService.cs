using Xango.Models.Dto;
using Xango.Services.Dto;

namespace Xango.Services.ShoppingCartAPI.Service.IService
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetProducts();
    }
}
