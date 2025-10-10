using Xango.Models.Dto;

namespace Xango.Services.Interfaces
{
    public interface IAuthService
    {
        Task<ResponseDto?> Login(LoginRequestDto loginRequestDto);
        //Task<ResponseDto?> Logout();
        Task<ResponseDto?> Register(RegistrationRequestDto registrationRequestDto);
        Task<ResponseDto?> AssignRole(RegistrationRequestDto registrationRequestDto);

        Task<ResponseDto> GetUser(string email);

        Task<ResponseDto?> Logout();
        
    }
}
