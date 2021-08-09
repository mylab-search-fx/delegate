using System.Threading.Tasks;
using MyLab.Search.Delegate.Models;

namespace MyLab.Search.Delegate.Services
{
    interface IEsFilterProvider
    {
        Task<SearchFilter> ProvideAsync(string filterId);
    }
}