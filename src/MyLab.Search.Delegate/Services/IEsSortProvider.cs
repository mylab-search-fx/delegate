using System.Collections.Generic;
using System.Threading.Tasks;
using MyLab.Search.Delegate.Models;
using Nest;

namespace MyLab.Search.Delegate.Services
{
    interface IEsSortProvider
    {
        Task<ISort> ProvideAsync(string sortId, string ns, IEnumerable<KeyValuePair<string, string>> args = null);
    }
}