using System.Threading.Tasks;
using MyLab.Search.Searcher.Models;
using Nest;
using FilterRef = MyLab.Search.Searcher.Models.FilterRef;

namespace MyLab.Search.Searcher.Services
{
    interface IEsRequestBuilder
    {
        Task<SearchRequest> BuildAsync(ClientSearchRequestV4 clientSearchRequest, string idxId, FilterRef[] filterRefs);
    }
}