using MyLab.Search.Delegate.Models;

namespace MyLab.Search.Delegate.Services
{
    public interface ITokenService
    {
        bool IsEnabled();
        string CreateSearchToken(TokenRequestV3 request);
        NamespaceSettingsV3 ValidateAndExtractSettings(string token, string ns);
    }
}
