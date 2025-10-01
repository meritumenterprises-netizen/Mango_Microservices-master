using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Xango.Models.Dto;
using Xango.Services.Dto.Utilities;
using Xango.Services.InventoryApi.Data;
using Xango.Services.InventoryApi.Service.IService;

namespace Xango.Services.InventoryApi.Controllers
{
    
    [Route("api/inventory")]
    [ApiController]
    public class ServiceIntentoryController : Controller
    {
        private IMapper _mapper;
        private IInventoryService _inventoryService;

        public ServiceIntentoryController(AppDbContext db, IMapper mapper, IInventoryService inventoryService)
        {
            _mapper = mapper;
            _inventoryService = inventoryService;
        }


        [HttpPost("setproductinstock")]
        public async Task<ResponseDto> SetProductInStock (int  productId, int stockSize)
        {
            try
            {
                var productDto = await _inventoryService.SetProductInStock(productId, stockSize);
                return ResponseProducer.OkResponse(productDto);
            }
            catch (Exception ex)
            {
                return ResponseProducer.ErrorResponse(ex.Message);
            }
        }

        [HttpGet("currentstock/{id:int}")]
        public async Task<ResponseDto> CurrentStock(int id)
        {
            try
            {
                var productInStock = await _inventoryService.CurrentStock(id);
                return ResponseProducer.OkResponse(productInStock);
            }
            catch (Exception ex)
            {
                return ResponseProducer.ErrorResponse(ex.Message);
            }
        }

        [HttpGet("instock/{id:int}")]
        public async Task<ResponseDto> IsProductInStock(int id)
        {
            try
            {
                var isProductInStock = await _inventoryService.IsProductInStock(id);
                return ResponseProducer.OkResponse(isProductInStock);
            }
            catch (Exception ex)
            {
                return ResponseProducer.ErrorResponse(ex.Message);
            }
        }

        [HttpPost("subtractfromstock")]
        public async Task<ResponseDto> SubtractFromStock(int productId, int quantity)
        {
            try
            {
                var productDto = await _inventoryService.SubtractFromStock(productId, quantity);
                return ResponseProducer.OkResponse(productDto);
            }
            catch (Exception ex)
            {
                return ResponseProducer.ErrorResponse(ex.Message);
            }
        }
    }
}
