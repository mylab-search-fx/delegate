using System.Threading.Tasks;
using Nest;

namespace MyLab.Search.Searcher.Services
{
    interface IIndexMappingService
    {
        Task<TypeMapping> GetIndexMappingAsync(string ns);
    }
}
