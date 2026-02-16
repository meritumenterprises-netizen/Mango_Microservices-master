
namespace Xango.Models.Dto
{
    public class OrderDetailsDto
    {
        public int OrderDetailsId { get; set; }
        public int OrderHeaderId { get; set; }
        public int ProductId { get; set; }
        public ProductDto? Product { get; set; } = null!;
        public int Count { get; set; }
        public string? ProductName { get; set; } = null!;
        public double Price { get; set; }
    }
}
