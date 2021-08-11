using System.Threading.Tasks;
using MyLab.Search.Delegate.Models;

namespace MyLab.Search.Delegate.Services
{
    interface IEsSortProvider
    {
        Task<SearchSort> ProvideAsync(string sortId, string ns);
    }
}