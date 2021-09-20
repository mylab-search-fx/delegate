using System.Threading.Tasks;
using MyLab.Search.Delegate.Models;
using Nest;
using FilterRef = MyLab.Search.Delegate.Models.FilterRef;

namespace MyLab.Search.Delegate.Services
{
    interface IEsRequestBuilder
    {
        Task<SearchRequest> BuildAsync(ClientSearchRequestV2 clientSearchRequest, string ns, FilterRef[] filterRefs);
    }
}