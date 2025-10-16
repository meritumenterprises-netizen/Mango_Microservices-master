using Xango.Services.ShoppingCartAPI.Service.IService;
using Newtonsoft.Json;
using Xango.Services.ShoppingCartAPI.Service.IService;
using Xango.Models.Dto;

namespace Xango.Services.ShoppingCartAPI.Service
{
    public class AuthService : IAuthService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public AuthService(IHttpClientFactory clientFactory)
        {
            _httpClientFactory = clientFactory;
        }

        public async Task<UserDto> GetUser(string userEmail)
        {
            //var client = _httpClientFactory.CreateClient("Auth");
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };

            var client = new HttpClient(handler);

            var response = await client.GetAsync($"/api/auth/GetUser/" + userEmail);
            var resp = response.Content.ReadFromJsonAsync<ResponseDto>().Result;
            if (resp != null && resp.IsSuccess)
            {
                return JsonConvert.DeserializeObject<UserDto>(Convert.ToString(resp.Result));
            }
            return new UserDto();
        }

        public async Task<UserDto> GetUserById(string id)
        {
            //var client = _httpClientFactory.CreateClient("Auth");
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };

            var client = new HttpClient(handler);

            var response = await client.GetAsync($"/api/auth/GetUserById/" + id);
            var resp = response.Content.ReadFromJsonAsync<ResponseDto>().Result;
            if (resp != null && resp.IsSuccess)
            {
                return JsonConvert.DeserializeObject<UserDto>(Convert.ToString(resp.Result));
            }
            return new UserDto();
        }

    }
}
