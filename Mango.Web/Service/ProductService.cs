using Xango.Models.Dto;
using Xango.Services.Interfaces;
using Xango.Web.Service.IService;
using Xango.Services.Client.Utility;

namespace Xango.Web.Service
{
    public class ProductService : IProductService
    {
        private readonly IBaseService _baseService;
        public ProductService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<ResponseDto?> CreateProducts(ProductDto productDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = ApiType.POST,
                Data = productDto,
                Url = SD.ProductAPIBase + "/api/product",
                ContentType = ContentType.MultipartFormData
            });
        }

        public async Task<ResponseDto?> DeleteProduct(int id)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = ApiType.DELETE,
                Url = SD.ProductAPIBase + "/api/product/" + id
            });
        }

        public async Task<ResponseDto?> GetAllProducts()
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = ApiType.GET,
                Url = SD.ProductAPIBase + "/api/product"
            });
        }



        public async Task<ResponseDto?> GetProductById(int id)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = ApiType.GET,
                Url = SD.ProductAPIBase + "/api/product/" + id
            });
        }

        public async Task<ResponseDto?> UpdateProducts(ProductDto productDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = ApiType.PUT,
                Data = productDto,
                Url = SD.ProductAPIBase + "/api/product",
                ContentType = ContentType.MultipartFormData
            });
        }
    }
}
