using Mango.Web.Models;
using Xango.Services.Dto;

namespace Mango.Web.Service.IService
{
    public interface IAuthService
    {
        Task<ResponseDto?> LoginAsync(LoginRequestDto loginRequestDto);
        Task<ResponseDto?> LogoutAsync(LogoutRequestDto logoutRequestDto);
        Task<ResponseDto?> RegisterAsync(RegistrationRequestDto registrationRequestDto);
        Task<ResponseDto?> AssignRoleAsync(RegistrationRequestDto registrationRequestDto);

        Task<ResponseDto?> GetUser(string email);
        Task<ResponseDto?> SetUser(string email, UserDto userDto);
        Task<UserDto> GetCurrentUser(HttpRequest request);
    }
}
