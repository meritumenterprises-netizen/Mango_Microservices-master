using Xango.Models.Dto;

namespace Xango.Services.Token
{
    public interface ITokenProvider
    {
        void SetToken(string token);
        string? GetToken();
        void ClearToken();
    }
}
