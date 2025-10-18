using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Security;
using System.Text;
using Xango.Models.Dto;
using Xango.Services.Client.Utility;
using Xango.Services.Dto;
using Xango.Services.Interfaces;
using Xango.Services.Client.Utility;
using Xango.Services.Server.Utility;
using Xango.Services.Utility;

namespace Xango.Service.CouponAPI.Client
{

    public class CouponHttpClient  : ICouponHttpClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly string _baseUri;
        private readonly ITokenProvider _tokenProvider;

        readonly string _baseAddress;
        public CouponHttpClient(IHttpClientFactory httpClientFactory, IConfiguration configuration, ITokenProvider tokenProvider)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _baseUri = Environment.GetEnvironmentVariable("CouponAPI");
            _tokenProvider = tokenProvider;
        }

        public async Task<ResponseDto?> CreateCoupons(CouponDto couponDto)
        {
            var client = _httpClientFactory.NewClientNoSslErrors("Coupon");
            client.BaseAddress = new Uri(_baseUri);
            var token = _tokenProvider.GetToken();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var resp = await client.PostAsync("/api/coupon", StringContentUTF8.AsJsonString<CouponDto>(couponDto));
            if (resp != null & resp.IsSuccessStatusCode)
            {
                return ResponseProducer.OkResponse(resp);
            }
            return ResponseProducer.ErrorResponse("Could not create a coupon");
        }

        public async Task<ResponseDto?> DeleteCoupons(int id)
        {
            var client = _httpClientFactory.NewClientNoSslErrors("Coupon");
            client.BaseAddress = new Uri(_baseUri);
            var token = _tokenProvider.GetToken();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = client.DeleteAsync("/api/coupon/" + id).GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();
            if (response != null & response.IsSuccessStatusCode)
            {
                return ResponseProducer.OkResponse(new ResponseDto());
            }
            return ResponseProducer.ErrorResponse("Could not delete a coupon");
        }

        public async Task<ResponseDto?> GetAllCoupons()
        {
            var client = _httpClientFactory.NewClientNoSslErrors("Coupon");
            client.BaseAddress = new Uri(_baseUri);
            var token = _tokenProvider.GetToken();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.GetAsync("/api/coupon",HttpCompletionOption.ResponseContentRead);
            response.EnsureSuccessStatusCode();
            var resp = response.Content.ReadFromJsonAsync<ResponseDto?>().GetAwaiter().GetResult();
            if (resp != null && resp.IsSuccess)
            {
                return ResponseProducer.OkResponse(resp?.Result);
            }
            return ResponseProducer.ErrorResponse("Could not find coupons");
        }

        public async Task<ResponseDto?> GetCoupon(string couponCode)
        {
            var client = _httpClientFactory.NewClientNoSslErrors("Coupon");
            client.BaseAddress = new Uri(_baseUri);
            var token = _tokenProvider.GetToken();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.GetAsync($"/api/coupon/GetByCode/{couponCode}");
            response.EnsureSuccessStatusCode();
            var resp = response.Content.ReadFromJsonAsync<ResponseDto>().Result;
            if (resp != null && resp.IsSuccess)
            {
                return ResponseProducer.OkResponse(resp);
            }
            return ResponseProducer.ErrorResponse("Could not find coupon");
        }

        public async Task<ResponseDto?> GetCouponById(int id)
        {
            var client = _httpClientFactory.NewClientNoSslErrors("Coupon");
            client.BaseAddress = new Uri(_baseUri);
            var token = _tokenProvider.GetToken();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.GetAsync($"/api/coupon/" + id);
            response.EnsureSuccessStatusCode();
            var resp = response.Content.ReadFromJsonAsync<ResponseDto>().Result;
            if (resp != null && resp.IsSuccess)
            {
                return ResponseProducer.OkResponse(resp);
            }
            return ResponseProducer.ErrorResponse("Could not find coupon");
        }
    }
}
