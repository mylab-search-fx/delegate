using MyLab.Search.Delegate.Models;
using NamespaceSettings = MyLab.Search.Delegate.Models.NamespaceSettings;
using TokenRequest = MyLab.Search.Delegate.Models.TokenRequest;

namespace MyLab.Search.Delegate.Services
{
    public interface ITokenService
    {
        bool IsEnabled();
        string CreateSearchToken(TokenRequest request);
        NamespaceSettings ValidateAndExtractSettings(string token, string ns);
    }
}
