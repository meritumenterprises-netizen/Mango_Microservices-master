using Xango.Models.Dto;

namespace Xango.Services.Interfaces
{
    public interface ICartService
    {
        Task<ResponseDto?> GetCartByUserId(string userId);
        Task<ResponseDto?> UpsertCart(CartDto cartDto);
        Task<ResponseDto?> RemoveFromCart(int cartDetailsId);
        Task<ResponseDto?> ApplyCoupon(CartDto cartDto);
        Task<ResponseDto?> EmailCart(CartDto cartDto);
        Task<ResponseDto> DeleteCart(string userId);
    }
}
