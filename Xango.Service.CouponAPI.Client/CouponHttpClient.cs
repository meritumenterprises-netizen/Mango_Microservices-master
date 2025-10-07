using Newtonsoft.Json;
using System.Net.Http.Json;
using Xango.Models.Dto;
using Xango.Services.Client.Utility;
using Xango.Services.Dto;
using Xango.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Xango.Service.CouponAPI.Client
{
    public interface ICouponHttpClient : ICouponService
    {

    }
    public class CouponHttpClient  : ICouponHttpClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly string _baseUri;

        readonly string _baseAddress;
        public CouponHttpClient(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _baseUri = _configuration["ServiceUrls:CouponAPI"];
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

        public async Task<ResponseDto?> GetCoupon(string couponCode)
        {
            var client = _httpClientFactory.CreateClient("Coupon");
            client.BaseAddress = new Uri(_baseUri);
            var response = await client.GetAsync($"/api/coupon/GetByCode/{couponCode}");
            var resp = response.Content.ReadFromJsonAsync<ResponseDto>().Result;
            if (resp != null && resp.IsSuccess)
            {
                return ResponseProducer.OkResponse(resp);
            }
            return ResponseProducer.ErrorResponse("Could not find coupon");
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
