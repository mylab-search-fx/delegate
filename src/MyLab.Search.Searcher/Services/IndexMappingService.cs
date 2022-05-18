using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyLab.Log;
using MyLab.Log.Dsl;
using MyLab.Search.EsAdapter;
using Nest;

namespace MyLab.Search.Searcher.Services
{
    class IndexMappingService : IIndexMappingService
    {
        private readonly SearcherOptions _esOptions;
        private readonly IEsClientProvider _esClientProvider;
        private readonly IDslLogger _log;
        private readonly ConcurrentDictionary<string, TypeMapping> _nsToIndexMapping = new ConcurrentDictionary<string, TypeMapping>();

        public IndexMappingService(
            IOptions<SearcherOptions> esOptions,
            IEsClientProvider esClientProvider,
            ILogger<IndexMappingService> logger = null)
            :this(esOptions.Value, esClientProvider, logger)
        {

        }

        public IndexMappingService(
            SearcherOptions esOptions,
            IEsClientProvider esClientProvider,
            ILogger<IndexMappingService> logger = null)
        {
            _esOptions = esOptions;
            _esClientProvider = esClientProvider;
            _log = logger?.Dsl();
        }

        public async Task<TypeMapping> GetIndexMappingAsync(string ns)
        {
            if (_nsToIndexMapping.TryGetValue(ns, out var currentMapping))
                return currentMapping;

            var indexName = _esOptions.GetIndexName(ns);

            var client = _esClientProvider.Provide();

            var mappingResponse = await client.Indices.GetMappingAsync(new GetMappingRequest(indexName));

            _log.Debug("Get index mapping")
                .AndFactIs("dump", ApiCallDumper.ApiCallToDump(mappingResponse.ApiCall))
                .Write();

            if (!mappingResponse.Indices.TryGetValue(indexName, out var indexMapping))
                throw new InvalidOperationException("Index mapping not found")
                    .AndFactIs("index", indexName);

            _nsToIndexMapping.TryAdd(ns, indexMapping.Mappings);

            return indexMapping.Mappings;
        }
    }
}