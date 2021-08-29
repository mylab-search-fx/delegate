using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyLab.Log.Dsl;
using MyLab.Search.Delegate.Models;
using MyLab.Search.Delegate.QueryStuff;
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

        public async Task<EsSearchRequest> BuildAsync(SearchRequest searchRequest, string ns, FiltersCall filtersCall)
        {
            var nsOptions = _options.GetNamespace(ns);

            int limit = searchRequest.Limit > 0
                ? searchRequest.Limit
                : nsOptions.DefaultLimit ?? 10;

            var req = new EsSearchRequest
            {
                Model = new EsSearchModel
                {
                    From = searchRequest.Offset,
                    Size = limit
                }
            };

            string sortId = searchRequest.Sort ?? nsOptions.DefaultSort;
            if (sortId != null)
            {
                var sort = await _esSortProvider.ProvideAsync(sortId, ns);
                req.Model.Sort = sort.Content;
            }

            var filtersToAdd = new List<SearchFilter>();

            string selectedFilterId = searchRequest.Filter ?? nsOptions.DefaultFilter;

            if (selectedFilterId != null)
            {
                var selectedFilter = await _filterProvider.ProvideAsync(selectedFilterId, ns);
                filtersToAdd.Add(selectedFilter);
            }

            if (filtersCall != null)
            {
                foreach (var filterCall in filtersCall)
                {
                    var filter = await _filterProvider.ProvideAsync(filterCall.Key, ns);
                    var initiator= new FilterInitiator(filterCall.Value);
                    initiator.InitFilter(filter);

                    filtersToAdd.Add(filter);
                }
            }
            
            var query = SearchQuery.Parse(searchRequest.Query);
            var mapping = await _indexMappingService.GetIndexMappingAsync(ns);

            var queryExpressions = GetQueryExpressions(query, mapping);

            if (filtersToAdd.Count != 0 || queryExpressions.Length != 0)
            {
                var boolModel = new EsSearchQueryBoolModel
                {
                    MinShouldMatch = query.IsEmpty ? null : (int?)1
                };

                if (filtersToAdd.Count != 0)
                    boolModel.Filter = filtersToAdd
                        .Select(f => f.Content)
                        .ToArray();

                if (queryExpressions.Length != 0)
                    boolModel.Should = queryExpressions;
                
                req.Model.Query = new EsSearchQueryModel
                {
                    Bool = boolModel
                };
            }

            return req;
        }

        string[] GetQueryExpressions(SearchQuery query, IndexMapping indexMapping)
        {
            var expressions = new List<string>();

            var unsupportedProperty = new List<IndexMappingProperty>();

            foreach (var prop in indexMapping.Props)
            {

                IReadOnlyCollection<ISearchQueryParam> qParams = null;

                switch (prop.Type)
                {
                    case "long":
                    case "integer":
                    case "short":
                    case "byte":
                    case "double":
                    case "float":
                    case "half_float":
                    case "scaled_float":
                    case "unsigned_long":
                        qParams = query.NumericParams;
                        break;
                    case "date":
                        qParams = query.DateTimeParams;
                        break;
                    case "text":
                    case "keyword":
                        qParams = query.TextParams;
                        break;
                }

                if (qParams == null)
                {
                    unsupportedProperty.Add(prop);
                }
                else
                {
                    expressions.AddRange(qParams.Select(param => param.ToJson(prop.Name, prop.Type)));
                }
            }

            if (unsupportedProperty.Count != 0)
            {
                _log.Warning("Met unsupported property types")
                    .AndFactIs("properties", unsupportedProperty)
                    .Write();
            }

            return expressions.Where(e => e != null).ToArray();
        }
    }
}