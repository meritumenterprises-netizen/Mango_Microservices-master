using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Xango.Models.Dto;
using Xango.Services.Dto.Utilities;

namespace Xango.Services.InventoryApi.Controllers
{
    [Route("api/inventory")]
    [ApiController]
    public class ServiceIntentoryController : Controller
    {
        [HttpPost("setproductinstock")]
        public async Task<ProductDto> SetProductInStock (int  productId, int stockSize)
        {
            var productInStock = new ProductDto()
            {

            };
            return productInStock;
        }

        [HttpGet("currentstock/{id:int}")]
        public async Task<int> CurrentStock(int id)
        {
            var productInStock = new ProductDto()
            {
            };
            return 1;
        }

        [HttpGet]
        public async Task<int> IsProductInStock(int productId)
        {
            return 1;
        }

        [HttpPost("subtractfromstock")]
        public async Task<ProductDto> SubtractFromStock(int productId, int quantity)
        {
            return new ProductDto()
            {
                StockInventory = 100
            };
        }
    }
}
