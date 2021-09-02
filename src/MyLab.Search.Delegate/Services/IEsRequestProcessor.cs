using System.Threading.Tasks;
using MyLab.Search.Delegate.Models;

namespace MyLab.Search.Delegate.Services
{
    public interface IEsRequestProcessor
    {
        Task<FoundEntities<FoundEntityContent>> ProcessSearchRequestAsync(SearchRequest request, string ns, string searchToken);
    }
}
