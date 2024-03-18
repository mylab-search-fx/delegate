using System.Threading.Tasks;
using MyLab.Search.Searcher;
using MyLab.Search.Searcher.Models;
using MyLab.Search.Searcher.Options;
using MyLab.Search.Searcher.Services;
using Nest;
using Xunit;

namespace UnitTests
{
    public partial class RequestBuilderBehavior
    {
        [Fact]
        public async Task ShouldBuildRequestByQuery()
        {
            //Arrange
            const string testQuery = "firstname middlename lastname 123";

            //Act
            var plan = await BuildRequestByQueryAsync(testQuery);

            //Assert
            Assert.NotNull(plan);
            Assert.Equal(QuerySearchStrategy.Must, plan.Query.Strategy);
            Assert.NotNull(plan.Query.QueryProcessor);
            
             Assert.Contains(plan.Query.QueryProcessor.Items, itm => itm is{ Name: testQuery, Boost:5} );
             Assert.Contains(plan.Query.QueryProcessor.Items, itm => itm is{ Name: "firstname", Boost:4} );
             Assert.Contains(plan.Query.QueryProcessor.Items, itm => itm is{ Name: "middlename", Boost:3} );
             Assert.Contains(plan.Query.QueryProcessor.Items, itm => itm is{ Name: "lastname", Boost:2} );
             Assert.Contains(plan.Query.QueryProcessor.Items, itm => itm is{ Name: "123", Boost:1} );
        }

        [Fact]
        public async Task ShouldPassIfIndexOptionsNotFound()
        {
            //Arrange
            var opt = new SearcherOptions
            {
                QueryStrategy = QuerySearchStrategy.Should
            };

            var reqBuilder = new EsRequestBuilder(opt,
                new TestSortProvider(),
                new TestFilterProvider(),
                new TestIndexMappingService());

            var sReq = new ClientSearchRequestV4()
            {
                Query = "nomater"
            };

            //Act
            var esReq = await reqBuilder.BuildRequestAsync(sReq, "test", null);
            var boolQuery = ((IQueryContainer)esReq.Query).Bool;


            //Assert
            Assert.NotNull(boolQuery.Should);
            Assert.Null(boolQuery.Must);
        }

        [Fact]
        public async Task ShouldUseDefaultQueryStrategyOr()
        {
            //Arrange
            var opt = new SearcherOptions
            {
                QueryStrategy = QuerySearchStrategy.Should,
                Indexes = new[]
                {
                    new IdxOptions
                    {
                        Id = "test"
                    }
                }
            };

            var reqBuilder = new EsRequestBuilder(opt,
                new TestSortProvider(),
                new TestFilterProvider(),
                new TestIndexMappingService());

            var sReq = new ClientSearchRequestV4()
            {
                Query = "nomater"
            };

            //Act
            var esReq = await reqBuilder.BuildRequestAsync(sReq, "test", null);
            var boolQuery = ((IQueryContainer)esReq.Query).Bool;


            //Assert
            Assert.NotNull(boolQuery.Should);
            Assert.Null(boolQuery.Must);
        }

        [Fact]
        public async Task ShouldUseDefaultQueryStrategyAnd()
        {
            //Arrange
            var opt = new SearcherOptions
            {
                QueryStrategy = QuerySearchStrategy.Must,
                Indexes = new[]
                {
                    new IdxOptions
                    {
                        Id = "test"
                    }
                }
            };

            var reqBuilder = new EsRequestBuilder(opt,
                new TestSortProvider(),
                new TestFilterProvider(),
                new TestIndexMappingService());

            var sReq = new ClientSearchRequestV4()
            {
                Query = "nomater"
            };

            //Act
            var esReq = await reqBuilder.BuildRequestAsync(sReq, "test", null);
            var boolQuery = ((IQueryContainer)esReq.Query).Bool;


            //Assert
            Assert.NotNull(boolQuery.Must);
            Assert.Null(boolQuery.Should);
        }

        [Fact]
        public async Task ShouldUseNamespaceQueryStrategy()
        {
            //Arrange
            var opt = new SearcherOptions
            {
                QueryStrategy = QuerySearchStrategy.Must,
                Indexes = new[]
                {
                    new IdxOptions
                    {
                        Id = "test",
                        QueryStrategy = QuerySearchStrategy.Should
                    }
                }
            };

            var reqBuilder = new EsRequestBuilder(opt,
                new TestSortProvider(),
                new TestFilterProvider(),
                new TestIndexMappingService());

            var sReq = new ClientSearchRequestV4()
            {
                Query = "nomater"
            };

            //Act
            var esReq = await reqBuilder.BuildRequestAsync(sReq, "test", null);
            var boolQuery = ((IQueryContainer)esReq.Query).Bool;


            //Assert
            Assert.NotNull(boolQuery.Should);
            Assert.Null(boolQuery.Must);
        }
    }
}
