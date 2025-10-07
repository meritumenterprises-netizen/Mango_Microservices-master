using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Xango.Services.InventoryApi.Data;
using Xango.Services.InventoryApi.Model;
using Xango.Services.InventoryApi.Service.IService;
using Microsoft.AspNetCore.Identity;
using Xango.Models.Dto;
using Xango.Services.Dto;

namespace Xango.Services.InventoryApi.Service
{
    public class InventoryService : IInventoryService
    {
        private readonly AppDbContext _db;
        private IMapper _mapper;
        public InventoryService ( AppDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }
        public async Task<int> CurrentStock(int productId)
        {
            var product = _db.Products.FirstOrDefault(p => p.ProductId == productId);
            if (product == null)
            {
                throw new Exception("No product with given id found");
            }
            return product.StockInventory;
        }

        public async Task<bool> IsProductInStock(int productId)
        {
            var product = _db.Products.FirstOrDefault(p => p.ProductId == productId);
            if (product == null)
            {
                throw new Exception("No product with given id found");
            }
            return product.StockInventory > 0;
        }

        public async Task<ProductDto> SetProductInStock(int productId, int quantity)
        {
            var product = _db.Products.FirstOrDefault(p => p.ProductId == productId);
            if (product == null)
            {
                throw new Exception("No product with given id found");
            }
            product.StockInventory = quantity;
            _db.SaveChanges();
            product = _db.Products.FirstOrDefault(p => p.ProductId == productId);
            var productDto = _mapper.Map<ProductDto>(product);  
            return productDto;
        }

        public async Task<ProductDto> SubtractFromStock(int productId, int quantity)
        {
            var product = _db.Products.FirstOrDefault(p => p.ProductId == productId);
            if (product == null)
            {
                throw new Exception("No product with given id found");
            }
            if (product.StockInventory < quantity)
            {
                throw new Exception("Insufficient product quantity");
            }
            product.StockInventory -= quantity;
            _db.SaveChanges();
            product = _db.Products.FirstOrDefault(p => p.ProductId == productId);
            var productDto = _mapper.Map<ProductDto>(product);
            return productDto;
        }

        public async Task<ProductDto> ReturnQty(int productId, int quantity)
        {
            var product = _db.Products.FirstOrDefault(p => p.ProductId == productId);
            if (product == null)
            {
                throw new Exception("No product with given id found");
            }
            product.StockInventory += quantity;
            _db.SaveChanges();
            product = _db.Products.FirstOrDefault(p => p.ProductId == productId);
            var productDto = _mapper.Map<ProductDto>(product);
            return productDto;
        }
    }
}
