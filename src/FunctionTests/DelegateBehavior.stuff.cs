using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyLab.ApiClient;
using MyLab.ApiClient.Test;
using MyLab.Search.Delegate;
using MyLab.Search.Delegate.Models;
using MyLab.Search.Delegate.Services;
using MyLab.Search.EsAdapter;
using MyLab.Search.EsTest;
using Xunit;
using Xunit.Abstractions;

namespace FunctionTests
{
    public partial class DelegateBehavior :
        IClassFixture<EsIndexFixture<TestEntity, TestConnectionProvider>>,
        IAsyncLifetime
    {
        private readonly EsIndexFixture<TestEntity, TestConnectionProvider> _esFxt;
        private readonly ITestOutputHelper _output;
        private readonly TestApi<Startup, ISearchService> _client;

        public DelegateBehavior(EsIndexFixture<TestEntity, TestConnectionProvider> esFxt, ITestOutputHelper output)
        {
            _esFxt = esFxt;
            //_esFxt.Output = output;

            _output = output;

            _client = new TestApi<Startup, ISearchService>()
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
                        o.Namespaces = new[]
                        {
                            new DelegateOptions.Namespace
                            {
                                Name = "test",
                                Index = esFxt.IndexName
                            }
                        };
                    })
                    .AddLogging(l => l
                        .AddXUnit(output)
                        .AddFilter(l => true)),
                Output = output
            };

            output.WriteLine("Test index: " + esFxt.IndexName);
        }

        public async Task InitializeAsync()
        {
            await _esFxt.Indexer.IndexManyAsync(CreateTestEntities());

            await Task.Delay(2000);
        }

        public async Task DisposeAsync()
        {
            _client.Dispose();
        }
    }
}