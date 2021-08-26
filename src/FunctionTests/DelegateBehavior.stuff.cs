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


    }
}