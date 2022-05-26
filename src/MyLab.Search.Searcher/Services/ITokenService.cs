using MyLab.Search.Searcher.Models;

namespace MyLab.Search.Searcher.Services
{
    public interface ITokenService
    {
        bool IsEnabled();
        string CreateSearchToken(TokenRequestV4 request);
        IndexSettingsV4 ValidateAndExtractSettings(string token, string idxId);
    }
}
