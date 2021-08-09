using System;
using System.Linq;
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
        private readonly ElasticsearchOptions _esOptions;
        private readonly IEsClientProvider _esClientProvider;
        private readonly IDslLogger _log;
        private IndexMapping _indexMapping;

        public IndexMappingService(
            IOptions<ElasticsearchOptions> esOptions,
            IEsClientProvider esClientProvider,
            ILogger<IndexMappingService> logger = null)
            :this(esOptions.Value, esClientProvider, logger)
        {

        }

        public IndexMappingService(
            ElasticsearchOptions esOptions,
            IEsClientProvider esClientProvider,
            ILogger<IndexMappingService> logger = null)
        {
            _esOptions = esOptions;
            _esClientProvider = esClientProvider;
            _log = logger?.Dsl();
        }

        public async Task<IndexMapping> GetIndexMappingAsync()
        {
            if (_indexMapping != null)
                return _indexMapping;

            var client = _esClientProvider.Provide();

            var mappingResponse = await client.Indices.GetMappingAsync(new GetMappingRequest(_esOptions.DefaultIndex));

            _log.Debug("Get index mapping")
                .AndFactIs("dump", ApiCallDumper.ApiCallToDump(mappingResponse.ApiCall))
                .Write();

            if (!mappingResponse.Indices.TryGetValue(_esOptions.DefaultIndex, out var indexMapping))
                throw new InvalidOperationException("Index mapping not found")
                    .AndFactIs("index", _esOptions.DefaultIndex);

            var propertiesMapping = indexMapping?.Mappings?.Properties;

            if (propertiesMapping == null)
                throw new InvalidOperationException("Index properties mapping not found")
                    .AndFactIs("index", _esOptions.DefaultIndex);

            _indexMapping = new IndexMapping(propertiesMapping.Values.Select(p => new IndexMappingProperty(p.Name.Name, p.Type)));

            return _indexMapping;
        }
    }
}