using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyLab.Search.Delegate.Models;

namespace MyLab.Search.Delegate.Services
{
    public interface IEsRequestProcessor
    {
        Task<IEnumerable<EsIndexedEntity>> ProcessSearchRequestAsync(SearchRequest request);
    }
}
