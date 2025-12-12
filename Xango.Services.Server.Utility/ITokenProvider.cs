using Xango.Models.Dto;

namespace Xango.Services.Server.Utility
{
    public interface ITokenProvider
    {
        void SetToken(string token);
        string? GetToken();
        void ClearToken();
    }
}
