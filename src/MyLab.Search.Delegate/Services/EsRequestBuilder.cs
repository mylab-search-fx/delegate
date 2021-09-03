using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyLab.Log.Dsl;
using MyLab.Search.Delegate.Models;
using MyLab.Search.Delegate.QueryTools;
using Nest;

namespace MyLab.Search.Delegate.Services
{
    class EsRequestBuilder : IEsRequestBuilder
    {
        private readonly DelegateOptions _options;
        private readonly IEsSortProvider _esSortProvider;
        private readonly IEsFilterProvider _filterProvider;
        private readonly IIndexMappingService _indexMappingService;
        private readonly IDslLogger _log;

        public EsRequestBuilder(
            IOptions<DelegateOptions> options, 
            IEsSortProvider esSortProvider,
            IEsFilterProvider filterProvider,
            IIndexMappingService indexMappingService,
            ILogger<EsRequestBuilder> logger = null)
        : this(options.Value, esSortProvider, filterProvider, indexMappingService, logger)
        {

        }
        public EsRequestBuilder(DelegateOptions options,
            IEsSortProvider esSortProvider,
            IEsFilterProvider filterProvider, 
            IIndexMappingService indexMappingService,
            ILogger<EsRequestBuilder> logger = null)
        {
            _options = options;
            _esSortProvider = esSortProvider ?? throw new ArgumentNullException(nameof(esSortProvider));
            _filterProvider = filterProvider ?? throw new ArgumentNullException(nameof(filterProvider));
            _indexMappingService = indexMappingService ?? throw new ArgumentNullException(nameof(indexMappingService));
            _log = logger?.Dsl();
        }

        public async Task<SearchRequest> BuildAsync(ClientSearchRequest clientSearchRequest, string ns, FiltersCall filtersCall)
        {
            var nsOptions = _options.GetNamespace(ns);

            int limit = clientSearchRequest.Limit > 0
                ? clientSearchRequest.Limit
                : nsOptions.DefaultLimit ?? 10;

            var req = new SearchRequest
            {
                From = clientSearchRequest.Offset,
                Size = limit
            };

            var filtersToAdd = new List<QueryContainer>();
            
            string selectedFilterId = clientSearchRequest.Filter ?? nsOptions.DefaultFilter;

            if (selectedFilterId != null)
            {
                var selectedFilter = await _filterProvider.ProvideAsync(selectedFilterId, ns);
                filtersToAdd.Add(selectedFilter);
            }

            if (filtersCall != null)
            {
                foreach (var filterCall in filtersCall)
                {
                    var filter = await _filterProvider.ProvideAsync(filterCall.Key, ns, filterCall.Value);
                    filtersToAdd.Add(filter);
                }
            }
            
            var queryProc = SearchQueryProcessor.Parse(clientSearchRequest.Query);
            var mapping = await _indexMappingService.GetIndexMappingAsync(ns);

            var queryExpressions = queryProc.Process(mapping).ToArray();

            bool hasFilters = filtersToAdd.Count != 0;
            bool hasQueries = queryExpressions.Length != 0;

            if (hasFilters || hasQueries)
            {
                var boolModel = new BoolQuery();

                if (hasFilters)
                    boolModel.Filter = filtersToAdd;

                if (hasQueries)
                {
                    DelegateOptions.QuerySearchStrategy queryStrategy = nsOptions.QueryStrategy != DelegateOptions.QuerySearchStrategy.Undefined 
                        ? nsOptions.QueryStrategy 
                        : _options.QueryStrategy;

                    if (queryStrategy == DelegateOptions.QuerySearchStrategy.Must)
                    {
                        boolModel.Must = queryExpressions;
                    }
                    else
                    {
                        boolModel.Should = queryExpressions;
                        boolModel.MinimumShouldMatch = 1;
                    }
                }

                req.Query = boolModel;
            }

            string sortId = clientSearchRequest.Sort ?? nsOptions.DefaultSort;
            if (sortId != null)
            {
                var sort = await _esSortProvider.ProvideAsync(sortId, ns);
                var sorts = new List<ISort> { sort };

                if (req.Query != null && clientSearchRequest.Sort == null)
                {
                    sorts.Insert(0, new FieldSort
                    {
                        Field = "_score",
                        Order = SortOrder.Descending
                    });
                }
                
                req.Sort = sorts;
            }

            return req;
        }
    }
}