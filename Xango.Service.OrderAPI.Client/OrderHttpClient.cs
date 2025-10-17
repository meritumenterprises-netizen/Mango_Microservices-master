using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xango.Models.Dto;
using Microsoft.Extensions.Configuration;
using Xango.Services.Server.Utility;
using System.Net.Http.Json;
using Xango.Services.Utility;
using Xango.Services.Client.Utility;

namespace Xango.Service.OrderAPI.Client
{
    public class OrderHttpClient : IOrderHttpClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly string _baseUri;
        private readonly ITokenProvider _tokenProvider;

        public OrderHttpClient(IHttpClientFactory httpClientFactory, IConfiguration configuration, ITokenProvider tokenProvider)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _baseUri = Environment.GetEnvironmentVariable("OrderAPI");
            _tokenProvider = tokenProvider;
        }
        public async Task<ResponseDto?> CreateOrder(CartDto cartDto)
        {
            var client = _httpClientFactory.NewClientNoSslErrors("Order");
            client.BaseAddress = new Uri(_baseUri);
            var token = _tokenProvider.GetToken();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = client.PostAsync("/api/order/CreateOrder", StringContentUTF8.AsJsonString(cartDto)).GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();
            var resp = response.Content.ReadFromJsonAsync<ResponseDto?>().GetAwaiter().GetResult();
            if (resp != null && resp.IsSuccess)
            {
                return ResponseProducer.OkResponse(resp.Result);
            }
            return ResponseProducer.ErrorResponse("Could not create order");

        }

        public async Task<ResponseDto?> CreateStripeSession(StripeRequestDto stripeRequestDto)
        {
            var client = _httpClientFactory.NewClientNoSslErrors("Order");
            client.BaseAddress = new Uri(_baseUri);
            var token = _tokenProvider.GetToken();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = client.PostAsync("/api/order/CreateStripeSession", StringContentUTF8.AsJsonString(stripeRequestDto)).GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();
            var resp = response.Content.ReadFromJsonAsync<ResponseDto?>().GetAwaiter().GetResult();
            if (resp != null && resp.IsSuccess)
            {
                return ResponseProducer.OkResponse(resp.Result);
            }
            return ResponseProducer.ErrorResponse("Could not create Stripe session");

        }

        public async Task<ResponseDto?> DeleteOrder(int orderId)
        {
            var client = _httpClientFactory.NewClientNoSslErrors("Order");
            client.BaseAddress = new Uri(_baseUri);
            var token = _tokenProvider.GetToken();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = client.DeleteAsync("/api/order/DeleteOrder/" + orderId).GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();
            var resp = response.Content.ReadFromJsonAsync<ResponseDto?>().GetAwaiter().GetResult();
            if (resp != null && resp.IsSuccess)
            {
                return ResponseProducer.OkResponse(resp.Result);
            }
            return ResponseProducer.ErrorResponse("Could not find delete order");
        }

        public async Task<ResponseDto?> GetAll(string userId, string status)
        {
            var client = _httpClientFactory.NewClientNoSslErrors("Order");
            client.BaseAddress = new Uri(_baseUri);
            var token = _tokenProvider.GetToken();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = client.GetAsync("/api/order/GetAll?" + $"userId={userId}&status={status}").GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();
            var resp = response.Content.ReadFromJsonAsync<ResponseDto?>().GetAwaiter().GetResult();
            if (resp != null && resp.IsSuccess)
            {
                return ResponseProducer.OkResponse(resp.Result);
            }
            return ResponseProducer.ErrorResponse("Could not find orders");

        }

        public async Task<ResponseDto?> GetOrder(int orderId)
        {
            var client = _httpClientFactory.NewClientNoSslErrors("Order");
            client.BaseAddress = new Uri(_baseUri);
            var token = _tokenProvider.GetToken();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = client.GetAsync("/api/order/GetOrder/" + orderId).GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();
            var resp = response.Content.ReadFromJsonAsync<ResponseDto?>().GetAwaiter().GetResult();
            if (resp != null && resp.IsSuccess)
            {
                return ResponseProducer.OkResponse(resp.Result);
            }
            return ResponseProducer.ErrorResponse("Could not find order");
        }

        public async Task<ResponseDto?> UpdateOrderStatus(int orderId, string newStatus)
        {
            var client = _httpClientFactory.NewClientNoSslErrors("Order");
            client.BaseAddress = new Uri(_baseUri);
            var token = _tokenProvider.GetToken();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = client.PostAsync("/api/order/UpdateOrderStatus/" + orderId,new StringContent("\"" + newStatus + "\"", Encoding.UTF8, "application/json")).GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();
            var resp = response.Content.ReadFromJsonAsync<ResponseDto?>().GetAwaiter().GetResult();
            if (resp != null && resp.IsSuccess)
            {
                return ResponseProducer.OkResponse(resp.Result);
            }
            return ResponseProducer.ErrorResponse("Could not find update order status");
        }

        public async Task<ResponseDto?> ValidateStripeSession(int orderHeaderId)
        {
            var client = _httpClientFactory.NewClientNoSslErrors("Order");
            client.BaseAddress = new Uri(_baseUri);
            var token = _tokenProvider.GetToken();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = client.PostAsync("/api/order/ValidateStripeSession", new StringContent(orderHeaderId.ToString(),Encoding.UTF8, "application/json")).GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();
            var resp = response.Content.ReadFromJsonAsync<ResponseDto?>().GetAwaiter().GetResult();
            if (resp != null && resp.IsSuccess)
            {
                return ResponseProducer.OkResponse(resp.Result);
            }
            return ResponseProducer.ErrorResponse("Could not validate Stripe session");

        }
    }
}
