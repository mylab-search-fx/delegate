using System.Threading.Tasks;
using MyLab.Search.Delegate.Models;

namespace MyLab.Search.Delegate.Services
{
    interface ISortProvider
    {
        Task<SearchSort> ProvideAsync(string name);
    }
}