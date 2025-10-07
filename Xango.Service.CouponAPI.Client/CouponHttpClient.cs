using Xango.Models.Dto;
using Xango.Services.Interfaces;

namespace Xango.Service.CouponAPI.Client
{
    public interface ICouponHttpClient
    {

    }
    public class CouponHttpClient  : HttpClient, ICouponService
    {
        readonly string _baseAddress;
        public CouponHttpClient(string baseAddress) 
        {
            _baseAddress = baseAddress;
        }

        public Task<ResponseDto?> CreateCoupons(CouponDto couponDto)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseDto?> DeleteCoupons(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseDto?> GetAllCoupons()
        {
            throw new NotImplementedException();
        }

        public Task<ResponseDto?> GetCoupon(string couponCode)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseDto?> GetCouponById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseDto?> UpdateCoupons(CouponDto couponDto)
        {
            throw new NotImplementedException();
        }
    }
}
