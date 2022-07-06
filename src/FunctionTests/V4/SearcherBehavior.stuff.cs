using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyLab.ApiClient.Test;
using MyLab.Search.Searcher;
using MyLab.Search.Searcher.Client;
using MyLab.Search.EsAdapter;
using MyLab.Search.EsTest;
using MyLab.Search.Searcher;
using MyLab.Search.Searcher.Options;
using MyLab.Search.Searcher.Services;
using MyLab.Search.SearcherClient;
using Nest;
using Xunit;
using Xunit.Abstractions;

namespace FunctionTests.V4
{
    public partial class SearcherBehavior :
        IClassFixture<EsIndexFixture<TestEntity, TestConnectionProvider>>,
        IAsyncLifetime
    {
        private readonly EsIndexFixture<TestEntity, TestConnectionProvider> _esFxt;
        private readonly ITestOutputHelper _output;
        private readonly TestApi<Startup, ISearcherApiV4> _searchClient;

        public SearcherBehavior(EsIndexFixture<TestEntity, TestConnectionProvider> esFxt, ITestOutputHelper output)
        {
            _esFxt = esFxt;
            //_esFxt.Output = output;

            _output = output;

            _searchClient = new TestApi<Startup, ISearcherApiV4>()
            {
                ServiceOverrider = ServiceOverrider,
                Output = output
            };

            output.WriteLine("Test index: " + esFxt.IndexName);
        }

        private void ServiceOverrider(IServiceCollection srv)
        {
            srv
                .Configure<ElasticsearchOptions>(o =>
                {
                    o.Url = "http://localhost:9200";
                })
                .Configure<SearcherOptions>(o =>
                {
                    o.FilterPath = "files/filter";
                    o.SortPath = "files/sort";
                    o.Indexes = new[]
                    {
                        new IdxOptions
                        {
                            Id = "test",
                            EsIndex = _esFxt.IndexName
                        }
                    };  
                })
                .AddLogging(l => l
                    .AddXUnit(_output)
                    .AddFilter(l => true));
        }

        public async Task InitializeAsync()
        {
            await _esFxt.Indexer.IndexManyAsync(CreateTestEntities());

            await Task.Delay(1000);
        }

        public async Task DisposeAsync()
        {
            _searchClient.Dispose();
        }

        class TestFilterProvider : IEsFilterProvider
        {
            private readonly QueryBase _query;

            public TestFilterProvider(QueryBase query)
            {
                _query = query;
            }

            public Task<QueryContainer> ProvideAsync(string filterId, string ns, IEnumerable<KeyValuePair<string, string>> args = null)
            {
                return Task.FromResult(new QueryContainer(_query));
            }
        }

        class TestSortProvider : IEsSortProvider
        {
            private readonly ISort _sort;

            public TestSortProvider(ISort sort)
            {
                _sort = sort;
            }

            public Task<ISort> ProvideAsync(string sortId, string ns, IEnumerable<KeyValuePair<string, string>> args = null)
            {
                return Task.FromResult(_sort);
            }

            public Task<ISort> ProvideDefaultAsync(string ns)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}