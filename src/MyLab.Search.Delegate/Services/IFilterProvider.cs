using System.Threading.Tasks;
using MyLab.Search.Delegate.Models;

namespace MyLab.Search.Delegate.Services
{
    interface IFilterProvider
    {
        Task<SearchFilter> ProvideAsync(string name);
    }
}