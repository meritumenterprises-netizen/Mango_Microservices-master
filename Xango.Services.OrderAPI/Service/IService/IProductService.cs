
using Xango.Services.Dto;
using Xango.Models.Dto;

namespace Xango.Services.ProductAPI.Service.IService
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetProducts();

    }
}
