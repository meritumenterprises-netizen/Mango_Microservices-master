using Xango.Models.Dto;

namespace Mango.Services.AuthAPI.Service.IService
{
    public interface IAuthService
    {
        Task<ResponseDto> Register(RegistrationRequestDto registrationRequestDto);
        Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto);
        Task<ResponseDto> CurrentUser (string email);
        Task<ResponseDto> AssignRole(string email, string roleName);
    }
}
