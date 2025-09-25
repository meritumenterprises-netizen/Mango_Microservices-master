using Mango.Services.ShoppingCartAPI.Models.Dto;
using Mango.Services.ShoppingCartAPI.Service.IService;
using Newtonsoft.Json;
using Xango.Services.Dto;
using Xango.Services.ShoppingCartAPI.Service.IService;

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
            var client = _httpClientFactory.CreateClient("Auth");
            var response = await client.GetAsync($"/api/auth/GetUser?email=" + userEmail);
            var resp = response.Content.ReadFromJsonAsync<ResponseDto>().Result;
            if (resp != null && resp.IsSuccess)
            {
                return JsonConvert.DeserializeObject<UserDto>(Convert.ToString(resp.Result));
            }
            return new UserDto();
        }

    }
}
