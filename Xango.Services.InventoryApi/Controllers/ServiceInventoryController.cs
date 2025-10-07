using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Xango.Models.Dto;
using Xango.Services.Dto;
using Xango.Services.Client.Utility;
using Xango.Services.InventoryApi.Data;
using Xango.Services.InventoryApi.Service.IService;

namespace Xango.Services.InventoryApi.Controllers
{
    
    [Route("api/inventory")]
    [ApiController]
    public class ServiceInventoryController : Controller
    {
        private AppDbContext _db;
        private IMapper _mapper;
        private IInventoryService _inventoryService;
        
        public ServiceInventoryController(AppDbContext db, IMapper mapper, IInventoryService inventoryService)
        {
            _mapper = mapper;
            _inventoryService = inventoryService;
            _db = db;
        }

        [HttpPost("returnqty")]
        public async Task<ResponseDto> ReturnQty(InventoryQuantityDto quantity)
        {
            try
            {
                var productDto = await _inventoryService.ReturnQty(quantity.ProductId, quantity.Quantity);
                return ResponseProducer.OkResponse(productDto);
            }
            catch (Exception ex)
            {
                return ResponseProducer.ErrorResponse(ex.Message);
            }
        }

        [HttpPost("setproductinstock")]
        public async Task<ResponseDto> SetProductInStock (InventoryQuantityDto inventoryQuantity)
        {
            try
            {
                var productDto = await _inventoryService.SetProductInStock(inventoryQuantity.ProductId, inventoryQuantity.Quantity);
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
        public async Task<ResponseDto> SubtractFromStock(InventoryQuantityDto inventoryQuantity)
        {
            try
            {
                var productDto = await _inventoryService.SubtractFromStock(inventoryQuantity.ProductId,inventoryQuantity.Quantity);
                return ResponseProducer.OkResponse(productDto);
            }
            catch (Exception ex)
            {
                return ResponseProducer.ErrorResponse(ex.Message);
            }
        }
    }
}
