using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Xango.Models.Dto
{
    public class ProductDto
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
        public string CategoryName { get; set; }
        public string? ImageUrl { get; set; }
        public string? ImageLocalPath { get; set; }
        public IFormFile? Image { get; set; }
        public int Count { get; set; } = 1;
<<<<<<< HEAD

        [Range(0,1000, ErrorMessage = "Inventory must be between 0 and 1000")]
        public int StockInventory { get; set; }

=======
>>>>>>> parent of 5887489 (Skeleton ServiceInventoryApi service)
    }
}
