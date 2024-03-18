using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyLab.ApiClient.Test;
using MyLab.Search.Searcher;
using MyLab.Search.SearcherClient;
using MyLab.Search.EsAdapter;
using MyLab.Search.EsTest;
using MyLab.Search.Searcher.Options;
using Xunit;
using Xunit.Abstractions;
using MyLab.Search.EsAdapter.Tools;
using Nest;

namespace FunctionTests.V4
{
    partial class QueryProcessingBehavior :
        IClassFixture<EsFixture<TestEsFixtureStrategy>>,
        IAsyncLifetime
    {
        private readonly EsFixture<TestEsFixtureStrategy> _esFxt;
        private readonly ITestOutputHelper _output;
        private readonly TestApi<Startup, ISearcherApiV4> _client;

        public QueryProcessingBehavior(EsFixture<TestEsFixtureStrategy> esFxt,
            ITestOutputHelper output)
        {
            _esFxt = esFxt;
            _esFxt.Output = output;

            _output = output;

            _client = new TestApi<Startup, ISearcherApiV4>()
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

        ISearcherApiV4 StartApi(string indexName)
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

        Task<IAsyncDisposable> CreateIndexAsync(string indexName)
            => CreateIndexAsync<TestEntity>(indexName);

        async Task<IAsyncDisposable> CreateIndexAsync<T>(string indexName) where T : class
        {
            var indexTools = await _esFxt.Tools.Indexes.CreateAsync
            (
                new CreateIndexDescriptor(indexName).Map(m => m.AutoMap<T>())
            );

            return TestTools.IndexToolToAsyncDeleter(indexTools);
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