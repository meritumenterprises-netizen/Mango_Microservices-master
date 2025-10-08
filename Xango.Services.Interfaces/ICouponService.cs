using Xango.Models.Dto;

namespace Xango.Services.Interfaces
{
    public interface ICouponService
    {
        Task<ResponseDto?> GetCoupon(string couponCode);
        Task<ResponseDto?> GetAllCoupons();
        Task<ResponseDto?> GetCouponById(int id);
        Task<ResponseDto?> CreateCoupons(CouponDto couponDto);
        Task<ResponseDto?> DeleteCoupons(int id);
    }
}
