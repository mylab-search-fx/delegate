using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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

        public async Task<SearchRequest> BuildRequestAsync(SearchRequestPlan plan, string idxId)
        {
            SearchRequest req = new SearchRequest
            {
                From = plan.From,
                Size = plan.Size
            };

            var sorts = new List<ISort>();

            if (plan.HasQuery)
            {
                var boolModel = new BoolQuery();

                if (plan.Query?.Filters is { Length: > 0 })
                    boolModel.Filter = await LoadFiltersAsync(plan.Query.Filters, idxId);

                if (plan.Query?.QueryApplier != null)
                {
                    var mapping = await _indexMappingService.GetIndexMappingAsync(idxId);
                    plan.Query.QueryApplier.Apply(mapping, plan.Query.Strategy, boolModel);
                }

                req.Query = boolModel;

                sorts.Add(new FieldSort
                {
                    Field = "_score",
                    Order = SortOrder.Descending
                });
            }

            if (plan.Sorting != null)
            {
                var sorting = await _esSortProvider.ProvideAsync(plan.Sorting.Id, idxId, plan.Sorting.Args);
                sorts.Add(sorting);
            }
            else
            {
                var sorting = await _esSortProvider.ProvideDefaultAsync(idxId);
                if(sorting != null)
                    sorts.Add(sorting);
            }

            req.Sort = sorts;

            return req;
        }

        public SearchRequestPlan BuildPlan(ClientSearchRequestV4 clientSearchRequest, string idxId, FilterRef[] filterRefs)
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
                : idxOptions?.DefaultLimit ?? 10;

            var req = new SearchRequestPlan
            {
                From = clientSearchRequest.Offset,
                Size = limit
            };


            var filtersToAdd = CompileFilters(clientSearchRequest.Filters, filterRefs, idxOptions?.DefaultFilter);

            var queryProc = SearchQueryApplier.Parse(clientSearchRequest.Query);

            bool hasFilters = filtersToAdd.Length != 0;
            bool hasQueries = queryProc.Items.Count != 0;

            if (hasFilters || hasQueries)
            {
                var queryModel = new SearchRequestPlanQuery();

                if (hasFilters)
                    queryModel.Filters = filtersToAdd;

                if (hasQueries)
                {
                    queryModel.Strategy = CalcSearchStrategy(clientSearchRequest, idxOptions);
                    queryModel.QueryApplier = queryProc;
                }

                req.Query = queryModel;
            }

            string sortId = clientSearchRequest.Sort?.Id ?? idxOptions?.DefaultSort;

            if (sortId != null)
            {
                req.Sorting = new SortingRef
                {
                    Id = sortId,
                    Args = clientSearchRequest.Sort?.Args
                };
            }
            
            return req;
        }

        public Task<SearchRequest> BuildRequestAsync(ClientSearchRequestV4 clientSearchRequest, string idxId, FilterRef[] filterRefs)
        {
            var plan = BuildPlan(clientSearchRequest, idxId, filterRefs);

            return BuildRequestAsync(plan, idxId);
        }

        private FilterRef[] CompileFilters(FilterRef[] requestFilters, FilterRef[] tokenFilters, string nsOptionsDefaultFilter)
        {
            var fc = new List<FilterRef>();

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
                if (nsOptionsDefaultFilter != null)
                    fc.Add(new FilterRef { Id = nsOptionsDefaultFilter });
            }

            return fc.ToArray();
        }

        private async Task<QueryContainer[]> LoadFiltersAsync(IEnumerable<FilterRef> filters, string indexId)
        {
            var result = new List<QueryContainer>();

            foreach (var f in filters)
            {
                var filter = await _filterProvider.ProvideAsync(f.Id, indexId, f.Args);
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
                if (idxOptions != null && idxOptions.QueryStrategy != default)
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