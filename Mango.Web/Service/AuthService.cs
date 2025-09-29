using Mango.Web.Service.IService;
using Xango.Services.Interfaces;
using Mango.Web.Utility;
using Newtonsoft.Json;
using Xango.Models.Dto;
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

        public async Task<ResponseDto?> GetUser(string email)
        {
            var requestDto= await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.GET,
                Data = "",
                Url = SD.AuthAPIBase + "/api/auth/GetUser/" + email
            });
            return ResponseProducer.OkResponse(requestDto.Result);
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

        public async Task<ResponseDto?> CurrentUser()
        {
            throw new NotImplementedException();
        }
    }
}
