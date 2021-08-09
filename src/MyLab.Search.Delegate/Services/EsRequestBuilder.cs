using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MyLab.Search.Delegate.Models;
using MyLab.Search.Delegate.Tools;
using Nest;
using SearchRequest = MyLab.Search.Delegate.Models.SearchRequest;

namespace MyLab.Search.Delegate.Services
{
    class EsRequestBuilder : IEsRequestBuilder
    {
        private readonly DelegateOptions _options;
        private readonly IEsSortProvider _esSortProvider;
        private readonly IEsFilterProvider _filterProvider;

        public EsRequestBuilder(
            IOptions<DelegateOptions> options, 
            IEsSortProvider esSortProvider,
            IEsFilterProvider filterProvider)
        : this(options.Value, esSortProvider, filterProvider)
        {

        }
        public EsRequestBuilder(
            DelegateOptions options, 
            IEsSortProvider esSortProvider,
            IEsFilterProvider filterProvider)
        {
            _options = options;
            _esSortProvider = esSortProvider ?? throw new ArgumentNullException(nameof(esSortProvider));
            _filterProvider = filterProvider ?? throw new ArgumentNullException(nameof(filterProvider));
        }

        public async Task<EsSearchRequest> BuildAsync(SearchRequest searchRequest)
        {
            int limit = searchRequest.Limit > 0
                ? searchRequest.Limit
                : _options.DefaultLimit ?? 10;

            var req = new EsSearchRequest
            {
                Model = new EsSearchModel
                {
                    From = searchRequest.Offset,
                    Size = limit
                }
            };

            string sortId = searchRequest.Sort ?? _options.DefaultSort;
            if (sortId != null)
            {
                var sort = await _esSortProvider.ProvideAsync(sortId);
                req.Model.Sort = sort.Content;
            }

            string filterId = searchRequest.Filter ?? _options.DefaultFilter;


            SearchFilter filter = null;

            if(filterId != null)
                filter = await _filterProvider.ProvideAsync(filterId);
            //else
            //{
            //    filter = SearchFilter.MatchAll;
            //}

            if (filter != null)
            {
                req.Model.Query = new EsSearchQueryModel
                {
                    Bool = new EsSearchQueryBoolModel
                    {
                        Must = new[]
                        {
                            filter.Content
                        }
                    }
                };
            }

            return req;
        }
    }
}