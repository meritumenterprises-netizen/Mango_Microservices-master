using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
            _baseUri = Environment.GetEnvironmentVariable("ProductAPI");
            _tokenProvider = tokenProvider;

        }
        public async Task<ResponseDto?> CreateProducts(ProductDto productDto)
        {
            var client = _httpClientFactory.NewClientNoSslErrors("Product");
            client.BaseAddress = new Uri(_baseUri);
            var token = _tokenProvider.GetToken();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.PostAsync("/api/product", StringContentUTF8.AsJsonString(productDto));
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
            var client = _httpClientFactory.NewClientNoSslErrors("Product");
            client.BaseAddress = new Uri(_baseUri);
            var token = _tokenProvider.GetToken();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.PutAsync("/api/product", StringContentUTF8.AsJsonString(productDto));
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
            var client = _httpClientFactory.NewClientNoSslErrors("Product");
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
            var client = _httpClientFactory.NewClientNoSslErrors("Product");
            client.BaseAddress = new Uri(_baseUri);
            
            var token = _tokenProvider.GetToken();
            if (token != null)
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            // this is a Retry microservices design pattern
            HttpResponseMessage response = null;
            var error = true;
            for (int i = 0; i < 5; i++)
            {
                try
                {
                    response = await client.GetAsync("/api/product");
                    error = false;
                }
                catch
                {
                    error = true;
                    Thread.Sleep(2000);
                }
                if (!error)
                {
                    break;
                }
            }
            response.EnsureSuccessStatusCode();
            var resp = await response.Content.ReadFromJsonAsync<ResponseDto?>();
            if (resp != null && resp.IsSuccess)
            {
                return ResponseProducer.OkResponse(resp.Result);
            }
            return ResponseProducer.ErrorResponse("Could not find products");
        }

        public async Task<ResponseDto?> GetProductById(int id)
        {
            var client = _httpClientFactory.NewClientNoSslErrors("Product");
            client.BaseAddress = new Uri(_baseUri);
            var token = _tokenProvider.GetToken();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.GetAsync("/api/product/" + id, HttpCompletionOption.ResponseContentRead);
            response.EnsureSuccessStatusCode();
            var resp = await response.Content.ReadFromJsonAsync<ResponseDto?>();
            if (resp != null && resp.IsSuccess)
            {
                return ResponseProducer.OkResponse(resp.Result);
            }
            return ResponseProducer.ErrorResponse("Could not find product");
        }
    }
}
