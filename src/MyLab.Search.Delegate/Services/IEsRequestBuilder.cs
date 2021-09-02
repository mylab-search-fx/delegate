using System.Threading.Tasks;
using MyLab.Search.Delegate.Models;
using Nest;

namespace MyLab.Search.Delegate.Services
{
    interface IEsRequestBuilder
    {
        Task<SearchRequest> BuildAsync(ClientSearchRequest clientSearchRequest, string ns, FiltersCall filterCall);
    }
}