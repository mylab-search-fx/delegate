using System.Threading.Tasks;
using MyLab.Search.Delegate.Models;

namespace MyLab.Search.Delegate.Services
{
    public interface IEsRequestProcessor
    {
        Task<FoundEntities<FoundEntityContent>> ProcessSearchRequestAsync(ClientSearchRequestV2 clientRequest, string ns, string searchToken);
    }
}
