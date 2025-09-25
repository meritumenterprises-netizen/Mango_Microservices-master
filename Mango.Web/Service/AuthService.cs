using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;
using Newtonsoft.Json;
using System.Web.Providers.Entities;
using Xango.Services.Dto;

namespace Mango.Web.Service
{
    public class AuthService : IAuthService
    {
        private readonly IBaseService _baseService;
        public AuthService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<UserDto> GetCurrentUser(HttpRequest request)
        {
            return new UserDto()
            {
                Email = request.HttpContext.User.Identity.Name
            };
        }

        public async Task<UserDto?> GetUser(string email)
        {
            var responseDto = await _baseService.SendAsync(new RequestDto
            {
                ApiType = SD.ApiType.GET,
                Data = "",
                Url = SD.AuthAPIBase + "/api/auth/GetUser/" + email
            });
            var userDto = JsonConvert.DeserializeObject<UserDto>(Convert.ToString(responseDto?.Result.ToString()));
            return userDto;
        }

        public async Task<ResponseDto?> AssignRoleAsync(RegistrationRequestDto registrationRequestDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = registrationRequestDto,
                Url = SD.AuthAPIBase + "/api/auth/AssignRole"
            });
        }

        public async Task<ResponseDto?> LoginAsync(LoginRequestDto loginRequestDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = loginRequestDto,
                Url = SD.AuthAPIBase + "/api/auth/login"
            }, withBearer: false);
        }

        public async Task<ResponseDto?> RegisterAsync(RegistrationRequestDto registrationRequestDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = registrationRequestDto,
                Url = SD.AuthAPIBase + "/api/auth/register"
            }, withBearer: false);
        }

        public async Task<ResponseDto?> LogoutAsync(LogoutRequestDto logoutRequestDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = logoutRequestDto,
                Url = SD.AuthAPIBase + "/api/auth/logout"
            });
        }
    }
}
