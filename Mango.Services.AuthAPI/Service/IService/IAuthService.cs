using Mango.Services.AuthAPI.Models.Dto;
using Xango.Services.Dto;

namespace Mango.Services.AuthAPI.Service.IService
{
    public interface IAuthService
    {
        Task<string> Register(RegistrationRequestDto registrationRequestDto);
        Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto);
        Task<UserDto> CurrentUser (string email);
        Task<bool> AssignRole(string email, string roleName);
    }
}
