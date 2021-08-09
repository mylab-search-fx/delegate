using System.Threading.Tasks;

namespace MyLab.Search.Delegate.Services
{
    interface IIndexMappingService
    {
        Task<IndexMapping> GetIndexMappingAsync();
    }
}
