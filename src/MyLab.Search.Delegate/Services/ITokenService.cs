using MyLab.Search.Delegate.Models;

namespace MyLab.Search.Delegate.Services
{
    public interface ITokenService
    {
        bool IsEnabled();
        string CreateSearchToken(TokenRequest request);
        NamespaceSettings ValidateAndExtractSettings(string token, string ns);
    }
}
