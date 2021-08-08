using System;
using System.Threading.Tasks;
using MyLab.Search.Delegate.Models;

namespace MyLab.Search.Delegate.Services
{
    class EsRequestBuilder : IEsRequestBuilder
    {
        public Task<EsSearchRequest> BuildAsync(SearchRequest searchRequest)
        {
            throw new NotImplementedException();
        }
    }
}