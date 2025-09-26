using Xango.Models.Dto;

namespace Mango.Web.Service.IService
{
    public interface IAuthService
    {
        Task<ResponseDto?> LoginAsync(LoginRequestDto loginRequestDto);
        Task<ResponseDto?> LogoutAsync(LogoutRequestDto logoutRequestDto);
        Task<ResponseDto?> RegisterAsync(RegistrationRequestDto registrationRequestDto);
        Task<ResponseDto?> AssignRoleAsync(RegistrationRequestDto registrationRequestDto);

        Task<UserDto> GetUser(string email);
        Task<UserDto> GetCurrentUser(HttpRequest request);
    }
}
