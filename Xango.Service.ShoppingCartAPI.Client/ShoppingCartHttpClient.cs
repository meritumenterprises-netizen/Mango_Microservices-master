using Microsoft.Extensions.Configuration;
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
            _baseUri = _configuration["ServiceUrls:ShoppingCartAPI"];
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

        public Task<ResponseDto?> GetCartByUserId(string userId)
        {
            var client = _httpClientFactory.NewClientNoSslErrors("ShoppingCart");
            client.BaseAddress = new Uri(_baseUri);
            var token = _tokenProvider.GetToken();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = client.GetAsync("/api/cart/GetCart/" + userId, HttpCompletionOption.ResponseContentRead).GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();
            var resp = response.Content.ReadFromJsonAsync<ResponseDto?>().GetAwaiter().GetResult();
            if (resp != null && resp.IsSuccess)
            {
                return Task.FromResult(ResponseProducer.OkResponse(resp.Result));
            }
            return Task.FromResult(ResponseProducer.ErrorResponse("Could not find cart"));
        }

        public Task<ResponseDto?> RemoveFromCart(int cartDetailsId)
        {
            var client = _httpClientFactory.NewClientNoSslErrors("ShoppingCart");
            client.BaseAddress = new Uri(_baseUri);
            var token = _tokenProvider.GetToken();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = client.PostAsync("/api/cart/RemoveCart/", new StringContent($"{cartDetailsId}", Encoding.UTF8, "application/json")).GetAwaiter().GetResult();
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
