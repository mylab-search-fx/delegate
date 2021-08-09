using System.Threading.Tasks;
using MyLab.Search.Delegate.Models;

namespace MyLab.Search.Delegate.Services
{
    class EsFilterProvider : IEsFilterProvider
    {
        public Task<SearchFilter> ProvideAsync(string filterId)
        {
            throw new System.NotImplementedException();
        }
    }
}