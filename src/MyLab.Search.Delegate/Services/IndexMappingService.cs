using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyLab.Log;
using MyLab.Log.Dsl;
using MyLab.Search.EsAdapter;
using Nest;

namespace MyLab.Search.Delegate.Services
{
    class IndexMappingService : IIndexMappingService
    {
        private readonly DelegateOptions _esOptions;
        private readonly IEsClientProvider _esClientProvider;
        private readonly IDslLogger _log;
        private readonly ConcurrentDictionary<string, TypeMapping> _nsToIndexMapping = new ConcurrentDictionary<string, TypeMapping>();

        public IndexMappingService(
            IOptions<DelegateOptions> esOptions,
            IEsClientProvider esClientProvider,
            ILogger<IndexMappingService> logger = null)
            :this(esOptions.Value, esClientProvider, logger)
        {

        }

        public IndexMappingService(
            DelegateOptions esOptions,
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

            var nsOptions = _esOptions.GetNamespace(ns);

            var client = _esClientProvider.Provide();

            var mappingResponse = await client.Indices.GetMappingAsync(new GetMappingRequest(nsOptions.Index));

            _log.Debug("Get index mapping")
                .AndFactIs("dump", ApiCallDumper.ApiCallToDump(mappingResponse.ApiCall))
                .Write();

            if (!mappingResponse.Indices.TryGetValue(nsOptions.Index, out var indexMapping))
                throw new InvalidOperationException("Index mapping not found")
                    .AndFactIs("index", nsOptions.Index);

            _nsToIndexMapping.TryAdd(ns, indexMapping.Mappings);

            return indexMapping.Mappings;
        }
    }
}