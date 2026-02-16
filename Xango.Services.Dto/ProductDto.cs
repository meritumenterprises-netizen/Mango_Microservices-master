using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Xango.Models.Dto
{
    public class ProductDto
    {
        public int ProductId { get; set; }
        public string? Name { get; set; } = null!;
        public double Price { get; set; }
        public string? Description { get; set; } = null!;
        public string? CategoryName { get; set; } = null!;
        public string? Base64Image { get; set; } = null!;
        public string? ImageUrl { get; set; } = null!;
        public string? ImageLocalPath { get; set; } = null!;
        [JsonIgnore]
        public IFormFile? Image { get; set; }
        public int Count { get; set; } = 1;
        [Range(0,1000, ErrorMessage = "Inventory must be between 0 and 1000")]
        public int StockInventory { get; set; }
    }
}
