using Xango.Models.Dto;


namespace Xango.Services.Interfaces
{
    public interface ICartService
    {
        Task<ResponseDto?> GetCartByUserId(string userId);
        Task<ResponseDto?> UpsertCart(CartDto cartDto);
        Task<ResponseDto?> ApplyCoupon(CartDto cartDto);
        Task<ResponseDto?> ApplyCoupon(ApplyCouponDto applyCouponDto);
        Task<ResponseDto?> RemoveCoupon(RemoveCouponDto userId);
		Task<ResponseDto?> EmailCart(CartDto cartDto);
        Task<ResponseDto> DeleteCart(string userId);

		Task<ResponseDto?> RemoveProductFromCart(int cartDetailsId);
		Task<ResponseDto> AddProductToCart(AddProductToCartDto addProductToCart);
	}
}
