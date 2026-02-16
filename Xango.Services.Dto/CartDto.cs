namespace Xango.Models.Dto
{
    public class CartDto
    {
        public CartHeaderDto CartHeader { get; set; } = null!;
        public List<CartDetailsDto>? CartDetails { get; set; } = null!;
    }
}
