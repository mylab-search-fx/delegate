using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elasticsearch.Net;
using MyLab.Search.Delegate.Models;
using MyLab.Search.Delegate.Tools;
using MyLab.Search.EsAdapter;
using Nest;
using SearchRequest = MyLab.Search.Delegate.Models.SearchRequest;

namespace MyLab.Search.Delegate.Services
{
    class EsRequestProcessor : IEsRequestProcessor
    {
        private readonly IEsRequestBuilder _requestBuilder;
        private readonly ElasticClient _esClient;
        private readonly EsSearchRequestSerializer _esReqSerializer;

        public EsRequestProcessor(
            IEsRequestBuilder requestBuilder,
            IEsClientProvider esClientProvider)
        {
            if (esClientProvider == null) throw new ArgumentNullException(nameof(esClientProvider));
            _requestBuilder = requestBuilder ?? throw new ArgumentNullException(nameof(requestBuilder));

            _esClient = esClientProvider.Provide();
            _esReqSerializer = new EsSearchRequestSerializer();
        }

        public async Task<IEnumerable<EsIndexedEntity>> ProcessSearchRequestAsync(SearchRequest request)
        {
            var esRequest = await _requestBuilder.BuildAsync(request);

            var strReq = _esReqSerializer.Serialize(esRequest.Model);
            var res = await _esClient.LowLevel.SearchAsync<SearchResponse<EsIndexedEntity>>(strReq);

            return res.Documents;
        }
    }
}