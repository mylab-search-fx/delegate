using System.Threading.Tasks;
using MyLab.Search.Delegate.Models;

namespace MyLab.Search.Delegate.Services
{
    interface IEsRequestBuilder
    {
        Task<EsSearchRequest> BuildAsync(SearchRequest searchRequest, string ns);
    }
}