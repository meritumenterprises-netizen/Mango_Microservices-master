
namespace Xango.Models.Dto
{
    public class OrderHeaderDto
    {
        public int OrderHeaderId { get; set; }
        public string? UserId { get; set; } = null!;
        public string? CouponCode { get; set; } = null!;
        public double Discount { get; set; }
        public double OrderTotal { get; set; }
        public string? OrderTotalWithCurrency { get; set; } = null!;
        public string? Name { get; set; } = null!;

		public string? Phone { get; set; } = null!;

        public string? Email { get; set; } = null!;
        public string? UserEmail { get; set; } = null!;
		public DateTime OrderTime { get; set; }
        public DateTime ModifiedTime { get; set; }
        public string? Status { get; set; } = null!;
        public string? PaymentIntentId { get; set; } = null!;
        public string? StripeSessionId { get; set; } = null!;
        public IEnumerable<OrderDetailsDto> OrderDetails { get; set; } = null!;
    }
}
