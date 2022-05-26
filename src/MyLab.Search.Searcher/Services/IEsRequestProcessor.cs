using System.Threading.Tasks;
using MyLab.Search.Searcher.Models;

namespace MyLab.Search.Searcher.Services
{
    public interface IEsRequestProcessor
    {
        Task<FoundEntities<FoundEntityContent>> ProcessSearchRequestAsync(ClientSearchRequestV4 clientRequest, string idxId, string searchToken);
    }
}
