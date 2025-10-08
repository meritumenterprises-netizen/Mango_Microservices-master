using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Text;
using Xango.Models.Dto;
using Xango.Services.Dto;
using Xango.Services.Client.Utility;
using Xango.Services.InventoryApi.Service.IService;
using Xango.Services.Utility;

namespace Xango.Services.InventoryApi.Service
{
    public class InventoryService : IInventoryService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public InventoryService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task<ResponseDto?> CurrentStock(int productId)
        {
            var client = _httpClientFactory.CreateClient("Inventory");
            var response = await client.GetAsync($"/api/inventory/currentstock/" + productId);
            return ResponseProducer.OkResponse(response);
        }

        public async Task<ResponseDto?> IsProductInStock(int productId)
        {
            var client = _httpClientFactory.CreateClient("Inventory");
            var response = await client.GetAsync($"/api/inventory/instock/" + productId);
            return ResponseProducer.OkResponse(response);
        }

        public async Task<ResponseDto?> ReturnQty(int productId, int quantity)
        {
            var client = _httpClientFactory.CreateClient("Inventory");
            var data = DtoConverter.ToJson(new InventoryQuantityDto() { ProductId = productId, Quantity = quantity });
            using var content = new StringContent(data, Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"/api/inventory/returnqty", content);
            return ResponseProducer.OkResponse(response);
        }

        public async Task<ResponseDto?> SetProductInStock(int productId, int quantity)
        {
            var client = _httpClientFactory.CreateClient("Inventory");
            var data = DtoConverter.ToJson(new InventoryQuantityDto() { ProductId = productId, Quantity = quantity });
            using var content = new StringContent(data, Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"/api/inventory/setproductinstock", content);
            return ResponseProducer.OkResponse(response);
        }

        public async Task<ResponseDto?> SubtractFromStock(int productId, int quantity)
        {
            var client = _httpClientFactory.CreateClient("Inventory");
            var data = DtoConverter.ToJson(new InventoryQuantityDto() { ProductId = productId, Quantity = quantity });
            using var content = new StringContent(data, Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"/api/inventory/subtractfromstock", content);
            return ResponseProducer.OkResponse(response);
        }
    }
}
