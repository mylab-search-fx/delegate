using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Elasticsearch.Net;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyLab.Log;
using MyLab.Log.Dsl;
using MyLab.Search.Delegate.Models;
using MyLab.Search.Delegate.Tools;
using MyLab.Search.EsAdapter;
using Nest;
using Newtonsoft.Json;
using SearchRequest = MyLab.Search.Delegate.Models.SearchRequest;

namespace MyLab.Search.Delegate.Services
{
    class EsRequestProcessor : IEsRequestProcessor
    {
        private readonly ElasticsearchOptions _options;
        private readonly IEsRequestBuilder _requestBuilder;
        private readonly ElasticClient _esClient;
        private readonly EsSearchRequestSerializer _esReqSerializer;
        private readonly IDslLogger _log;

        public EsRequestProcessor(
            IOptions<ElasticsearchOptions> options,
            IEsRequestBuilder requestBuilder,
            IEsClientProvider esClientProvider,
            ILogger<EsRequestProcessor> logger = null)
        :this(options.Value, requestBuilder, esClientProvider, logger)
        {

        }

        public EsRequestProcessor(
            ElasticsearchOptions options,
            IEsRequestBuilder requestBuilder,
            IEsClientProvider esClientProvider,
            ILogger<EsRequestProcessor> logger = null)
        {
            if (esClientProvider == null) throw new ArgumentNullException(nameof(esClientProvider));
            _options = options;
            _requestBuilder = requestBuilder ?? throw new ArgumentNullException(nameof(requestBuilder));

            _esClient = esClientProvider.Provide();
            _esReqSerializer = new EsSearchRequestSerializer();
            _log = logger?.Dsl();
        }

        public async Task<IEnumerable<EsIndexedEntity>> ProcessSearchRequestAsync(SearchRequest request)
        {
            var esRequest = await _requestBuilder.BuildAsync(request);

            var strReq = _esReqSerializer.Serialize(esRequest.Model);

            _log?.Debug("Perform search request")
                .AndFactIs("origin-req", request)
                .AndFactIs("search-req", strReq)
                .AndFactIs("index", _options.DefaultIndex)
                .Write();

            SearchResponse<EsIndexedEntityContent> res;
            try
            {
                res = await _esClient.LowLevel.SearchAsync<SearchResponse<EsIndexedEntityContent>>(_options.DefaultIndex, strReq);
            }
            catch (UnexpectedElasticsearchClientException e)
            {
                if(e.Response != null)
                    e.AndFactIs("dump", ApiCallDumper.ApiCallToDump(e.Response));

                throw;
            }

            return res.Hits.Select(h => new EsIndexedEntity
            {
                Content = h.Source,
                Score = h.Score
            });
        }
    }
}