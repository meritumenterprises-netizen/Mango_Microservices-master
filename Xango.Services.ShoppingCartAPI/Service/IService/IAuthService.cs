using Xango.Models.Dto;

namespace Xango.Services.ShoppingCartAPI.Service.IService
{
    public interface IAuthService
    {
        //UserDto GetUser(string email);
        Task<UserDto> GetUser(string userEmail);
        Task<UserDto> GetUserById(string id);
    }
}
