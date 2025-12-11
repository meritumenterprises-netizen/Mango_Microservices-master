using Xango.Models.Dto;

namespace Xango.Services.AuthAPI.Service.IService
{
    public interface IAuthService
    {
        Task<string> Register(RegistrationRequestDto registrationRequestDto);
        Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto);
        Task<bool> AssignRole(string email, string roleName);
        Task<bool> IsUserInRole(string email, string roleName);
        Task<UserDto> GetUser(string email);
        Task<UserDto> GetUserById(string id);
    }
}
