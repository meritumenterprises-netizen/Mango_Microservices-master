using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Xango.Models.Dto;
using Xango.Services.Client.Utility;
using Xango.Services.Dto;
using Xango.Services.Server.Utility;
using Xango.Services.Utility;

namespace Xango.Service.ShoppingCartAPI.Client
{
    public class ShoppingCartHttpClient : IShoppingCartHttpClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ITokenProvider _tokenProvider;
        private readonly string _baseUri;

        public ShoppingCartHttpClient(IHttpClientFactory httpClientFactory, IConfiguration configuration, ITokenProvider tokenProvider)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _tokenProvider = tokenProvider;
            _baseUri = Environment.GetEnvironmentVariable("ShoppingCartAPI");
        }

        public Task<ResponseDto> AddProductToCart(AddProductToCartDto addProductToCart)
        {
            var client = _httpClientFactory.NewClientNoSslErrors("ShoppingCart");
            client.BaseAddress = new Uri(_baseUri);
            var token = _tokenProvider.GetToken();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = client.PostAsync("/api/cart/AddProductToCart", StringContentUTF8.AsJsonString(addProductToCart)).GetAwaiter().GetResult();
			response.EnsureSuccessStatusCode();
			var resp = response.Content.ReadFromJsonAsync<ResponseDto?>().GetAwaiter().GetResult();
			if (resp != null && resp.IsSuccess)
			{
				return Task.FromResult(ResponseProducer.OkResponse(resp.Result));
			}
			return Task.FromResult(ResponseProducer.ErrorResponse("Could not find cart"));
		}

		public Task<ResponseDto?> ApplyCoupon(CartDto cartDto)
        {
            var client = _httpClientFactory.NewClientNoSslErrors("ShoppingCart");
            client.BaseAddress = new Uri(_baseUri);
            var token = _tokenProvider.GetToken();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = client.PostAsync("/api/cart/ApplyCoupon/", StringContentUTF8.AsJsonString(cartDto)).GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();
            var resp = response.Content.ReadFromJsonAsync<ResponseDto?>().GetAwaiter().GetResult();
            if (resp != null && resp.IsSuccess)
            {
                return Task.FromResult(ResponseProducer.OkResponse(resp.Result));
            }
            return Task.FromResult(ResponseProducer.ErrorResponse("Could not find cart"));
        }

		public Task<ResponseDto?> ApplyCoupon(ApplyCouponDto couponDto)
		{
			var client = _httpClientFactory.NewClientNoSslErrors("ShoppingCart");
			client.BaseAddress = new Uri(_baseUri);
			var token = _tokenProvider.GetToken();
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
			var response = client.PostAsync("/api/cart/ApplyCoupon/", StringContentUTF8.AsJsonString(couponDto)).GetAwaiter().GetResult();
			response.EnsureSuccessStatusCode();
			var resp = response.Content.ReadFromJsonAsync<ResponseDto?>().GetAwaiter().GetResult();
			if (resp != null && resp.IsSuccess)
			{
				return Task.FromResult(ResponseProducer.OkResponse(resp.Result));
			}
			return Task.FromResult(ResponseProducer.ErrorResponse("Could not find cart"));
		}

		public Task<ResponseDto?> RemoveCoupon(RemoveCouponDto userId)
		{
			var client = _httpClientFactory.NewClientNoSslErrors("ShoppingCart");
			client.BaseAddress = new Uri(_baseUri);
			var token = _tokenProvider.GetToken();
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
			var response = client.PostAsync("/api/cart/RemoveCoupon/", StringContentUTF8.AsJsonString(userId)).GetAwaiter().GetResult();
			response.EnsureSuccessStatusCode();
			var resp = response.Content.ReadFromJsonAsync<ResponseDto?>().GetAwaiter().GetResult();
			if (resp != null && resp.IsSuccess)
			{
				return Task.FromResult(ResponseProducer.OkResponse(resp.Result));
			}
			return Task.FromResult(ResponseProducer.ErrorResponse("Could not find cart"));
		}


		public Task<ResponseDto> DeleteCart(string userId)
        {
            var client = _httpClientFactory.NewClientNoSslErrors("ShoppingCart");
            client.BaseAddress = new Uri(_baseUri);
            var token = _tokenProvider.GetToken();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = client.DeleteAsync("/api/cart/DeleteCart/" + userId).GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();
            var resp = response.Content.ReadFromJsonAsync<ResponseDto?>().GetAwaiter().GetResult();
            if (resp != null && resp.IsSuccess)
            {
                return Task.FromResult(ResponseProducer.OkResponse(resp.Result));
            }
            return Task.FromResult(ResponseProducer.ErrorResponse("Could not find cart"));
        }

        public Task<ResponseDto?> EmailCart(CartDto cartDto)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseDto?> GetCartByUserId(string userId)
        {
            var client = _httpClientFactory.NewClientNoSslErrors("ShoppingCart");
            client.BaseAddress = new Uri(_baseUri);
            var token = _tokenProvider.GetToken();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.GetAsync("/api/cart/GetCart/" + userId);
            response.EnsureSuccessStatusCode();
            var resp = await response.Content.ReadFromJsonAsync<ResponseDto?>();
            if (resp != null && resp.IsSuccess)
            {
                return ResponseProducer.OkResponse(resp);
            }
            return ResponseProducer.ErrorResponse("Could not find cart");
        }

        public Task<ResponseDto?> RemoveProductFromCart(RemoveProductFromCartDto cartDetailsId)
        {
            var client = _httpClientFactory.NewClientNoSslErrors("ShoppingCart");
            client.BaseAddress = new Uri(_baseUri);
            var token = _tokenProvider.GetToken();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = client.PostAsync("/api/cart/RemoveCart/", new StringContent($"{cartDetailsId.CartDetailsId}", Encoding.UTF8, "application/json")).GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();
            var resp = response.Content.ReadFromJsonAsync<ResponseDto?>().GetAwaiter().GetResult();
            if (resp != null && resp.IsSuccess)
            {
                return Task.FromResult(ResponseProducer.OkResponse(resp.Result));
            }
            return Task.FromResult(ResponseProducer.ErrorResponse("Could not find cart"));
        }

        public Task<ResponseDto?> UpsertCart(CartDto cartDto)
        {
            var client = _httpClientFactory.NewClientNoSslErrors("ShoppingCart");
            client.BaseAddress = new Uri(_baseUri);
            var token = _tokenProvider.GetToken();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = client.PostAsync("/api/cart/CartUpsert/", StringContentUTF8.AsJsonString(cartDto)).GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();
            var resp = response.Content.ReadFromJsonAsync<ResponseDto?>().GetAwaiter().GetResult();
            if (resp != null && resp.IsSuccess)
            {
                return Task.FromResult(ResponseProducer.OkResponse(resp.Result));
            }
            return Task.FromResult(ResponseProducer.ErrorResponse("Could not find cart"));
        }
	}
}
