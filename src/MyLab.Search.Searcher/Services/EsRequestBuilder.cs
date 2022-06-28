using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyLab.Log.Dsl;
using MyLab.Search.Searcher.Models;
using MyLab.Search.Searcher.Options;
using MyLab.Search.Searcher.QueryTools;
using Nest;
using FilterRef = MyLab.Search.Searcher.Models.FilterRef;

namespace MyLab.Search.Searcher.Services
{
    class EsRequestBuilder : IEsRequestBuilder
    {
        private readonly SearcherOptions _options;
        private readonly IEsSortProvider _esSortProvider;
        private readonly IEsFilterProvider _filterProvider;
        private readonly IIndexMappingService _indexMappingService;
        private readonly IDslLogger _log;

        public EsRequestBuilder(
            IOptions<SearcherOptions> options, 
            IEsSortProvider esSortProvider,
            IEsFilterProvider filterProvider,
            IIndexMappingService indexMappingService,
            ILogger<EsRequestBuilder> logger = null)
        : this(options.Value, esSortProvider, filterProvider, indexMappingService, logger)
        {

        }
        public EsRequestBuilder(SearcherOptions options,
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

        public async Task<SearchRequest> BuildAsync(ClientSearchRequestV4 clientSearchRequest, string idxId, FilterRef[] filterRefs)
        {
            IdxOptions idxOptions;

            try
            {
                idxOptions = _options.GetIndexOptions(idxId);
            }
            catch (NamespaceConfigException e)
            {
                idxOptions = e.IndexOptionsFromNamespaceOptions;

                _log?.Warning(e).Write();
            }

            int limit = clientSearchRequest.Limit > 0
                ? clientSearchRequest.Limit
                : idxOptions.DefaultLimit ?? 10;

            var req = new SearchRequest
            {
                From = clientSearchRequest.Offset,
                Size = limit
            };


            var filtersToAdd = await LoadFilters(clientSearchRequest.Filters, filterRefs, idxOptions.DefaultFilter, idxId);
            
            var queryProc = SearchQueryProcessor.Parse(clientSearchRequest.Query);
            var mapping = await _indexMappingService.GetIndexMappingAsync(idxId);

            var queryExpressions = queryProc.Process(mapping).ToArray();

            bool hasFilters = filtersToAdd.Length != 0;
            bool hasQueries = queryExpressions.Length != 0;

            if (hasFilters || hasQueries)
            {
                var boolModel = new BoolQuery();

                if (hasFilters)
                    boolModel.Filter = filtersToAdd;

                if (hasQueries)
                {
                    var queryStrategy = CalcSearchStrategy(clientSearchRequest, idxOptions);

                    if (queryStrategy == QuerySearchStrategy.Must)
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

            var sorts = new List<ISort>();

            string sortId = clientSearchRequest.Sort?.Id ?? idxOptions.DefaultSort;

            if (sortId != null)
            {
                var sort = await _esSortProvider.ProvideAsync(sortId, idxId, clientSearchRequest.Sort?.Args);

                if (req.Query != null && clientSearchRequest.Sort == null)
                {
                    sorts.Insert(0, new FieldSort
                    {
                        Field = "_score",
                        Order = SortOrder.Descending
                    });

                }

                sorts.Add(sort);
            }
            else
            {
                var defaultSort = await _esSortProvider.ProvideDefaultAsync(idxId);

                if(defaultSort != null)
                    sorts.Add(defaultSort);
            }

            req.Sort = sorts;

            

            return req;
        }

        private async Task<QueryContainer[]> LoadFilters(FilterRef[] requestFilters, FilterRef[] tokenFilters, string nsOptionsDefaultFilter, string ns)
        {
            var fc  =new List<FilterRef>();

            if (tokenFilters != null && tokenFilters.Length != 0)
            {
                fc.AddRange(tokenFilters);
            }

            if (requestFilters != null && requestFilters.Length != 0)
            {
                fc.AddRange(requestFilters);
            }
            else
            {
                if(nsOptionsDefaultFilter != null)
                    fc.Add(new FilterRef { Id = nsOptionsDefaultFilter});
            }

            var result = new List<QueryContainer>();

            foreach (var fRef in fc)
            {
                var filter = await _filterProvider.ProvideAsync(fRef.Id, ns, fRef.Args);
                result.Add(filter);
            }

            return result.ToArray();
        }

        private QuerySearchStrategy CalcSearchStrategy(ClientSearchRequestV4 clientSearchRequest, IdxOptions idxOptions)
        {
            QuerySearchStrategy queryStrategy;

            if (clientSearchRequest.QuerySearchStrategy != default)
            {
                queryStrategy = clientSearchRequest.QuerySearchStrategy;
            }
            else
            {
                if (idxOptions.QueryStrategy != default)
                {
                    queryStrategy = idxOptions.QueryStrategy;
                }
                else
                {
                    queryStrategy = _options.QueryStrategy != default
                        ? _options.QueryStrategy
                        : QuerySearchStrategy.Should;
                }
            }

            return queryStrategy;
        }
    }
}