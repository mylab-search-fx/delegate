using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyLab.ApiClient.Test;
using MyLab.Search.Delegate;
using MyLab.Search.Delegate.Client;
using MyLab.Search.EsAdapter;
using MyLab.Search.EsTest;
using Xunit;
using Xunit.Abstractions;

namespace FunctionTests
{
    partial class QueryProcessingBehavior :
        IClassFixture<EsFixture<TestConnectionProvider>>,
        IAsyncLifetime
    {
        private readonly EsFixture<TestConnectionProvider> _esFxt;
        private readonly ITestOutputHelper _output;
        private readonly TestApi<Startup, ISearchDelegateApiV1> _client;

        public QueryProcessingBehavior(EsFixture<TestConnectionProvider> esFxt,
            ITestOutputHelper output)
        {
            _esFxt = esFxt;

            _output = output;

            _client = new TestApi<Startup, ISearchDelegateApiV1>()
            {
                ServiceOverrider = srv => srv
                    .Configure<ElasticsearchOptions>(o =>
                    {
                        o.Url = "http://localhost:9200";
                    })
                    .Configure<DelegateOptions>(o =>
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

        ISearchDelegateApiV1 StartApi(string indexName)
        {
            return _client.StartWithProxy(srv =>
            {
                srv.Configure<DelegateOptions>(o =>
                {
                    o.Debug = true;
                    o.Namespaces = new[]
                    {
                        new DelegateOptions.Namespace
                        {
                            Name = "test",
                            Index = indexName
                        }
                    };
                });
            });
        }

        string CreateIndexName() => "test-" + Guid.NewGuid().ToString("N");

        public async Task InitializeAsync()
        {
        }

        public async Task DisposeAsync()
        {
            _client.Dispose();
        }
    }
}