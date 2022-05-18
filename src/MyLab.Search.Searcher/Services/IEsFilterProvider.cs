using System.Collections.Generic;
using System.Threading.Tasks;
using Nest;

namespace MyLab.Search.Searcher.Services
{
    interface IEsFilterProvider
    {
        Task<QueryContainer> ProvideAsync(string filterId, string ns, IEnumerable<KeyValuePair<string,string>> args = null);
    }
}