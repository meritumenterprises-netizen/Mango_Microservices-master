
using Xango.Models.Dto;
namespace Xango.Services.CouponAPI.Service.IService
{
    public interface ICouponService
    {
        Task<CouponDto> GetCoupon(string couponCode);
    }
}
