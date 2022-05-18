using MyLab.Search.Searcher.Models;

namespace MyLab.Search.Searcher.Services
{
    public interface ITokenService
    {
        bool IsEnabled();
        string CreateSearchToken(TokenRequestV3 request);
        NamespaceSettingsV3 ValidateAndExtractSettings(string token, string ns);
    }
}
