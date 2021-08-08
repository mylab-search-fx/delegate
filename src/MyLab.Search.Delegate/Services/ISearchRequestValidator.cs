using System.Threading.Tasks;
using MyLab.Search.Delegate.Models;

namespace MyLab.Search.Delegate.Services
{
    interface ISearchRequestValidator
    {
        Task ValidateAsync(SearchRequest request);
    }

    class SearchRequestValidator : ISearchRequestValidator
    {
        public Task ValidateAsync(SearchRequest request)
        {
            throw new System.NotImplementedException();
        }
    }
}
