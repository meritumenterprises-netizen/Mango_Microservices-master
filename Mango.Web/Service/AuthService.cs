using Mango.Web.Service.IService;
using Xango.Services.Interfaces;
using Mango.Web.Utility;
using Newtonsoft.Json;
using Xango.Models.Dto;

namespace Mango.Web.Service
{
    public class AuthService : IAuthService
    {
        private readonly IBaseService _baseService;
        public AuthService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        //public async Task<ResponseDto?> GetCurrentUser(HttpRequest request)
        //{

        //    var userDto = new UserDto()
        //    {
        //        Email = request.HttpContext.User.Identity.Name
        //    };
        //    var responseDto = new ResponseDto()
        //    {
        //        IsSuccess = true,
        //        Result = JsonConvert.SerializeObject(userDto)
        //    };
        //    return responseDto;
        //}

        public async Task<ResponseDto?> GetUser(string email)
        {
            var requestDto= await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.GET,
                Data = "",
                Url = SD.AuthAPIBase + "/api/auth/GetUser/" + email
            });
            var responseDto = new ResponseDto()
            {
                IsSuccess = true,
                Result = requestDto?.Result.ToString()
            };
            return responseDto;
        }

        public async Task<ResponseDto?> AssignRole(Xango.Models.Dto.RegistrationRequestDto registrationRequestDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = ApiType.POST,
                Data = registrationRequestDto,
                Url = SD.AuthAPIBase + "/api/auth/AssignRole"
            });
        }

        public async Task<ResponseDto?> Login(Xango.Models.Dto.LoginRequestDto loginRequestDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = ApiType.POST,
                Data = loginRequestDto,
                Url = SD.AuthAPIBase + "/api/auth/login"
            }, withBearer: false);
        }

        public async Task<ResponseDto?> Register(Xango.Models.Dto.RegistrationRequestDto registrationRequestDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = ApiType.POST,
                Data = registrationRequestDto,
                Url = SD.AuthAPIBase + "/api/auth/register"
            }, withBearer: false);
        }

        public async Task<ResponseDto?> Logout()
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = ApiType.POST,
                Data = "",
                Url = SD.AuthAPIBase + "/api/auth/logout"
            });
        }

    }
}
