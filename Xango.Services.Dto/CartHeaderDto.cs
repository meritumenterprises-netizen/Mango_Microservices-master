namespace Xango.Models.Dto
{
    public class CartHeaderDto
    {
        public int CartHeaderId { get; set; }
        public string? UserId { get; set; } = null!;
        public string? CouponCode { get; set; } = null!;
        public double Discount { get; set; }
        public double CartTotal { get; set; }


        public string? Name { get; set; } = null!;

        public string? Phone { get; set; } = null!;

        public string? Email { get; set; } = null!;
    }
}
