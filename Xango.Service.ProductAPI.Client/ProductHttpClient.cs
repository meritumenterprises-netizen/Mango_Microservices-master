using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Newtonsoft.Json;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using Xango.Models.Dto;
using Xango.Services.Client.Utility;
using Xango.Services.Server.Utility;
using Xango.Services.Utility;
using Microsoft.AspNetCore.Http.Internal;
using Xango.Services.Dto;

namespace Xango.Service.ProductAPI.Client
{
    public class ProductHttpClient : IProductHttpClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly string _baseUri;
        private readonly ITokenProvider _tokenProvider;

        public ProductHttpClient(IHttpClientFactory httpClientFactory, IConfiguration configuration, ITokenProvider tokenProvider)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _baseUri = _configuration["ServiceUrls:ProductAPI"];
            _tokenProvider = tokenProvider;
        }
        public async Task<ResponseDto?> CreateProducts(ProductDto productDto)
        {
            var client = _httpClientFactory.CreateClient("Product");
            client.BaseAddress = new Uri(_baseUri);
            var token = _tokenProvider.GetToken();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = client.PostAsync("/api/product", StringContentUTF8.AsJsonString(productDto)).GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();
            var resp = response.Content.ReadFromJsonAsync<ResponseDto?>().GetAwaiter().GetResult();
            if (resp != null && resp.IsSuccess)
            {
                return ResponseProducer.OkResponse(resp.Result);
            }
            return ResponseProducer.ErrorResponse("Could not find product");
        }

        public async Task<ResponseDto?> UpdateProducts(ProductDto productDto)
        {
            var client = _httpClientFactory.CreateClient("Product");
            client.BaseAddress = new Uri(_baseUri);
            var token = _tokenProvider.GetToken();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = client.PutAsync("/api/product", StringContentUTF8.AsJsonString(productDto)).GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();
            var resp = response.Content.ReadFromJsonAsync<ResponseDto?>().GetAwaiter().GetResult();
            if (resp != null && resp.IsSuccess)
            {
                return ResponseProducer.OkResponse(resp.Result);
            }
            return ResponseProducer.ErrorResponse("Could not find product");
        }

        public async Task<ResponseDto?> DeleteProduct(int id)
        {
            var client = _httpClientFactory.CreateClient("Product");
            client.BaseAddress = new Uri(_baseUri);
            var token = _tokenProvider.GetToken();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = client.DeleteAsync("/api/product/" + id).GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();
            var resp = response.Content.ReadFromJsonAsync<ResponseDto?>().GetAwaiter().GetResult();
            if (resp != null && resp.IsSuccess)
            {
                return ResponseProducer.OkResponse(resp.Result);
            }
            return ResponseProducer.ErrorResponse("Could not find product");
        }

        public async Task<ResponseDto?> GetAllProducts()
        {
            var client = _httpClientFactory.CreateClient("Product");
            client.BaseAddress = new Uri(_baseUri);
            var token = _tokenProvider.GetToken();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = client.GetAsync("/api/product", HttpCompletionOption.ResponseContentRead).GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();
            var resp = response.Content.ReadFromJsonAsync<ResponseDto?>().GetAwaiter().GetResult();
            if (resp != null && resp.IsSuccess)
            {
                return ResponseProducer.OkResponse(resp.Result);
            }
            return ResponseProducer.ErrorResponse("Could not find products");
        }

        public async Task<ResponseDto?> GetProductById(int id)
        {
            var client = _httpClientFactory.CreateClient("Product");
            client.BaseAddress = new Uri(_baseUri);
            var token = _tokenProvider.GetToken();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = client.GetAsync("/api/product/" + id, HttpCompletionOption.ResponseContentRead).GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();
            var resp = response.Content.ReadFromJsonAsync<ResponseDto?>().GetAwaiter().GetResult();
            if (resp != null && resp.IsSuccess)
            {
                return ResponseProducer.OkResponse(resp.Result);
            }
            return ResponseProducer.ErrorResponse("Could not find product");
        }
    }
}
