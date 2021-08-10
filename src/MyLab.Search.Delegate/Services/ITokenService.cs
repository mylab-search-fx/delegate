using MyLab.Search.Delegate.Models;

namespace MyLab.Search.Delegate.Services
{
    public interface ITokenService
    {
        bool IsEnabled();
        string CreateSearchToken(TokenRequest request);
        FiltersCall ValidateAndExtractSearchToken(string token);
    }
}
