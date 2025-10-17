using Newtonsoft.Json;
using System.Net.Http.Json;
using Xango.Models.Dto;
using Xango.Services.Client.Utility;
using Xango.Services.Dto;
using Xango.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Text;
using Xango.Services.Server.Utility;
using System.Net.Http.Headers;
using System.Net.Http;
using Xango.Services.Utility;

namespace Xango.Service.InventoryAPI.Client
{
    public class InventoryHttpClient : IInventoryttpClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly string _baseUri;
        private readonly ITokenProvider _tokenProvider;

        public InventoryHttpClient(IHttpClientFactory httpClientFactory, IConfiguration configuration, ITokenProvider tokenProvider)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _baseUri = Environment.GetEnvironmentVariable("InventoryAPI");
            _tokenProvider = tokenProvider;
        }

        public async Task<ResponseDto?> CurrentStock(int productId)
        {
            var client = _httpClientFactory.NewClientNoSslErrors("Inventory");
            client.BaseAddress = new Uri(_baseUri);
            var token = _tokenProvider.GetToken();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = client.GetAsync($"/api/inventory/currentstock/{productId}").GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();
            var resp = response.Content.ReadFromJsonAsync<ResponseDto>().Result;
            if (resp != null && resp.IsSuccess)
            {
                return ResponseProducer.OkResponse(resp);
            }
            return ResponseProducer.ErrorResponse("Could not find product");
        }

        public async Task<ResponseDto?> IsProductInStock(int productId)
        {
            var client = _httpClientFactory.NewClientNoSslErrors("Inventory");
            client.BaseAddress = new Uri(_baseUri);
            var token = _tokenProvider.GetToken();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = client.GetAsync($"/api/inventory/instock/{productId}").GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();
            var resp = response.Content.ReadFromJsonAsync<ResponseDto>().Result;
            if (resp != null && resp.IsSuccess)
            {
                return ResponseProducer.OkResponse(resp);
            }
            return ResponseProducer.ErrorResponse("Could not find product");
        }

        public async Task<ResponseDto?> ReturnQty(int productId, int quantity)
        {
            var client = _httpClientFactory.NewClientNoSslErrors("Inventory");
            client.BaseAddress = new Uri(_baseUri);
            var token = _tokenProvider.GetToken();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = client.PostAsync($"/api/inventory/returnqty", StringContentUTF8.AsJsonString(new InventoryQuantityDto { ProductId = productId, Quantity = quantity })).GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();
            var resp = response.Content.ReadFromJsonAsync<ProductDto>().Result;
            if (resp != null)
            {
                return ResponseProducer.OkResponse(resp);
            }
            return ResponseProducer.ErrorResponse("Could not find product");
        }

        public async Task<ResponseDto?> SetProductInStock(int productId, int quantity)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseDto?> SubtractFromStock(int productId, int quantity)
        {
            var client = _httpClientFactory.NewClientNoSslErrors("Inventory");
            client.BaseAddress = new Uri(_baseUri);
            var token = _tokenProvider.GetToken();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = client.PostAsync($"/api/inventory/subtractfromstock", StringContentUTF8.AsJsonString(new InventoryQuantityDto { ProductId = productId, Quantity = quantity })).GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();
            var resp = response.Content.ReadFromJsonAsync<ProductDto>().Result;
            if (resp != null)
            {
                return ResponseProducer.OkResponse(resp);
            }
            return ResponseProducer.ErrorResponse("Could not find product");
        }
    }
}
