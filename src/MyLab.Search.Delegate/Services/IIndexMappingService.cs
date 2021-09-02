using System.Threading.Tasks;
using Nest;

namespace MyLab.Search.Delegate.Services
{
    interface IIndexMappingService
    {
        Task<TypeMapping> GetIndexMappingAsync(string ns);
    }
}
