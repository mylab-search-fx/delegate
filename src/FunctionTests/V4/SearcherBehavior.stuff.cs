using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyLab.ApiClient.Test;
using MyLab.Search.Searcher;
using MyLab.Search.SearcherClient;
using MyLab.Search.EsAdapter;
using MyLab.Search.EsAdapter.Indexing;
using MyLab.Search.EsTest;
using MyLab.Search.Searcher.Options;
using MyLab.Search.Searcher.Services;
using Nest;
using Xunit;
using Xunit.Abstractions;

namespace FunctionTests.V4
{
    public partial class SearcherBehavior :
        IClassFixture<EsIndexFixture<TestEntity, TestEsFixtureStrategy>>,
        IAsyncLifetime
    {
        private readonly EsIndexFixture<TestEntity, TestEsFixtureStrategy> _esFxt;
        private readonly ITestOutputHelper _output;
        private readonly TestApi<Startup, ISearcherApiV4> _searchClient;

        public SearcherBehavior(EsIndexFixture<TestEntity, TestEsFixtureStrategy> esFxt, ITestOutputHelper output)
        {
            _esFxt = esFxt;
            _esFxt.Output = output;

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
                .Configure<EsOptions>(o =>
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
            var bulkIndexingRequest = new EsBulkIndexingRequest<TestEntity>()
            {
                CreateList = CreateTestEntities().ToArray()
            };

            await _esFxt.Indexer.BulkAsync(bulkIndexingRequest);

            await Task.Delay(1000);
        }

        public Task DisposeAsync()
        {
            _searchClient.Dispose();
            return _esFxt.IndexTools.PruneAsync();
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