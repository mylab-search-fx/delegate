using System.Collections.Generic;
using System.Threading.Tasks;
using MyLab.Search.Delegate.Models;
using Nest;

namespace MyLab.Search.Delegate.Services
{
    interface IEsFilterProvider
    {
        Task<QueryContainer> ProvideAsync(string filterId, string ns, IEnumerable<KeyValuePair<string,string>> args = null);
    }
}