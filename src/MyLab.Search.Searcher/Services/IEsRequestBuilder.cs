using System.Threading.Tasks;
using MyLab.Search.Searcher.Models;
using Nest;
using FilterRef = MyLab.Search.Searcher.Models.FilterRef;

namespace MyLab.Search.Searcher.Services
{
    interface IEsRequestBuilder
    {
        Task<SearchRequest> BuildRequestAsync(ClientSearchRequestV4 clientSearchRequest, string idxId, FilterRef[] filterRefs);
        Task<SearchRequest> BuildRequestAsync(SearchRequestPlan requestPlan,string idxId);
        SearchRequestPlan BuildPlan(ClientSearchRequestV4 clientSearchRequest, string idxId, FilterRef[] filterRefs);
    }
}