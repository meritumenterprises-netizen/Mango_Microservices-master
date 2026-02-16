namespace Xango.Models.Dto
{
    public class StripeRequestDto
    {
        public string? StripeSessionUrl { get; set; } = null!;
        public string? StripeSessionId { get; set; } = null!;
		public string? ApprovedUrl { get; set; } = null!;
        public string? CancelUrl { get; set; } = null!;
        public OrderHeaderDto? OrderHeader { get; set; } = null!;
    }
}
