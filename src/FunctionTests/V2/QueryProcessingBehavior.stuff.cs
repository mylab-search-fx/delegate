using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyLab.ApiClient.Test;
using MyLab.Search.Searcher;
using MyLab.Search.EsAdapter;
using MyLab.Search.EsTest;
using MyLab.Search.Searcher.Options;
using MyLab.Search.SearcherClient;
using Xunit;
using Xunit.Abstractions;

namespace FunctionTests.V2
{
    partial class QueryProcessingBehavior :
        IClassFixture<EsFixture<TestEsFixtureStrategy>>,
        IAsyncLifetime
    {
        private readonly EsFixture<TestEsFixtureStrategy> _esFxt;
        private readonly ITestOutputHelper _output;
        private readonly TestApi<Startup, ISearcherApiV2> _client;

        public QueryProcessingBehavior(EsFixture<TestEsFixtureStrategy> esFxt,
            ITestOutputHelper output)
        {
            _esFxt = esFxt;

            _output = output;

            _client = new TestApi<Startup, ISearcherApiV2>()
            {
                ServiceOverrider = srv => srv
                    
                    .ConfigureEsTools(o =>
                    {
                        o.Url = "http://localhost:9200";
                    })
                    .Configure<SearcherOptions>(o =>
                    {
                        o.FilterPath = "files/filter";
                        o.SortPath = "files/sort";
                    })
                    .AddLogging(l => l
                        .AddXUnit(output)
                        .AddFilter(l => true)),
                Output = output
            };
        }

        ISearcherApiV2 StartApi(string indexName)
        {
            return _client.StartWithProxy(srv =>
            {
                srv.Configure<SearcherOptions>(o =>
                {
                    o.Debug = true;
                    o.Indexes = new[]
                    {
                        new IdxOptions
                        {
                            Id = "test",
                            EsIndex= indexName
                        }
                    };
                });
            });
        }

        string CreateIndexName() => "test-" + Guid.NewGuid().ToString("N");

        Task<IIndexDeleter> CreateIndexAsync(string indexName) => _esFxt.IndexTools.CreateIndexAsync(indexName, c => c.Map<TestEntity>(m => m.AutoMap()));
        Task<IIndexDeleter> CreateIndexAsync<T>(string indexName)
            where T : class
        {
            return _esFxt.IndexTools.CreateIndexAsync(indexName, c => c.Map<T>(m => m.AutoMap()));
        }


        public async Task InitializeAsync()
        {
        }

        public async Task DisposeAsync()
        {
            _client.Dispose();
        }
    }
}