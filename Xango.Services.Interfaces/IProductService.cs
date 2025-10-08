using Xango.Models.Dto;
using Xango.Services.Dto;

namespace Xango.Services.Interfaces
{
    public interface IProductService
    {
        Task<ResponseDto?> GetAllProducts();
        Task<ResponseDto?> GetProductById(int id);
        Task<ResponseDto?> CreateProducts(ProductDto productDto);
        Task<ResponseDto?> UpdateProducts(ProductDto productDto);
        Task<ResponseDto?> DeleteProduct(int id);
    }
}
