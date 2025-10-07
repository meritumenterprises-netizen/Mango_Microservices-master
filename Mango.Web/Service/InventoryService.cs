using Xango.Models.Dto;
using Xango.Services.Dto;
using Xango.Services.Interfaces;
using Xango.Web.BaseService;
using Xango.Web.Service.IService;


namespace Xango.Web.Service
{
    public class InventoryService : IInventoryService
    {
        private readonly IBaseService _baseService;
        public InventoryService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<ResponseDto?> CurrentStock(int productId)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = ApiType.GET,
                Url = SD.InventoryAPIBase+ "/api/inventory/currentstock/" + productId
            });
        }

        public async Task<ResponseDto?> IsProductInStock(int productId)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = ApiType.GET,
                Url = SD.InventoryAPIBase + "/api/inventory/instock/" + productId
            });
        }

        public async Task<ResponseDto?> SetProductInStock(int productId, int quantity)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = ApiType.POST,
                Data = new InventoryQuantityDto()
                {
                    ProductId = productId,
                    Quantity = quantity
                },
                Url = SD.InventoryAPIBase + "/api/inventory/setproductinstock"
            });
        }

        public async Task<ResponseDto?> SubtractFromStock(int productId, int quantity)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = ApiType.POST,
                Data = new InventoryQuantityDto()
                {
                    ProductId = productId,
                    Quantity = quantity
                },

                Url = SD.InventoryAPIBase + "/api/inventory/subtractfromstock"
            });
        }

        public async Task<ResponseDto?> ReturnQty(int productId, int quantity)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = ApiType.POST,
                Data = new InventoryQuantityDto()
                {
                    ProductId = productId,
                    Quantity = quantity
                },

                Url = SD.InventoryAPIBase + "/api/inventory/returnqty/" 
            });
        }
    }
}
