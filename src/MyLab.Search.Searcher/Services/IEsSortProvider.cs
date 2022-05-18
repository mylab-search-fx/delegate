using System.Collections.Generic;
using System.Threading.Tasks;
using Nest;

namespace MyLab.Search.Searcher.Services
{
    interface IEsSortProvider
    {
        Task<ISort> ProvideAsync(string sortId, string ns, IEnumerable<KeyValuePair<string, string>> args = null);
    }
}