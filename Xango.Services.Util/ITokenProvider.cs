using Xango.Models.Dto;

namespace Xango.Services.Utility
{
    public interface ITokenProvider
    {
        void SetToken(string token);
        string? GetToken();
        void ClearToken();
    }
}
