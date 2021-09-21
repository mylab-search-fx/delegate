using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyLab.ApiClient.Test;
using MyLab.Search.Delegate;
using MyLab.Search.Delegate.Client;
using MyLab.Search.Delegate.Services;
using MyLab.Search.EsAdapter;
using MyLab.Search.EsTest;
using Nest;
using Xunit;
using Xunit.Abstractions;
using FilterArgs = MyLab.Search.Delegate.Models.FilterArgs;

namespace FunctionTests.V1
{
    public partial class DelegateBehavior :
        IClassFixture<EsIndexFixture<TestEntity, TestConnectionProvider>>,
        IAsyncLifetime
    {
        private readonly EsIndexFixture<TestEntity, TestConnectionProvider> _esFxt;
        private readonly ITestOutputHelper _output;
        private readonly TestApi<Startup, ISearchDelegateApiV1> _searchClient;

        public DelegateBehavior(EsIndexFixture<TestEntity, TestConnectionProvider> esFxt, ITestOutputHelper output)
        {
            _esFxt = esFxt;
            //_esFxt.Output = output;

            _output = output;

            _searchClient = new TestApi<Startup, ISearchDelegateApiV1>()
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
                .Configure<DelegateOptions>(o =>
                {
                    o.FilterPath = "files/filter";
                    o.SortPath = "files/sort";
                    o.Namespaces = new[]
                    {
                        new DelegateOptions.Namespace
                        {
                            Name = "test",
                            Index = _esFxt.IndexName
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

            await Task.Delay(2000);
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

            public Task<ISort> ProvideAsync(string sortId, string ns)
            {
                return Task.FromResult(_sort);
            }
        }
    }
}