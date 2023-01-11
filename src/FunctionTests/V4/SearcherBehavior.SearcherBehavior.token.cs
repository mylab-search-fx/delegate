using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MyLab.ApiClient;
using MyLab.Search.SearcherClient;
using MyLab.Search.Searcher.Options;
using Xunit;

namespace FunctionTests.V4
{
    public partial class SearcherBehavior
    {
        [Theory]
        [InlineData("specified", "test")]
        [InlineData("default", "*")]
        public async Task ShouldUseFilterFromToken(string desc, string indexIdInToken)
        {
            //Arrange
            var cl = _searchClient.StartWithProxy(srv => srv.Configure<SearcherOptions>(o =>
            {
                o.Token = new TokenizingOptions
                {
                    SignKey = string.Join(';', Enumerable.Repeat(Guid.NewGuid().ToString("N"), 10))
                };
            }));

            var tokenRequest = new MyLab.Search.SearcherClient.TokenRequestV4()
            {
                Indexes = new[]
                {
                    new MyLab.Search.SearcherClient.IndexSettingsV4
                    {
                        Id = indexIdInToken,
                        Filters = new[]
                        {
                            new MyLab.Search.SearcherClient.FilterRef
                            {
                                Id = "from5to15"
                            }
                        }
                    }
                }
            };

            //Act
            string token = await cl.CreateSearchTokenAsync(tokenRequest);

            var found = await cl.SearchAsync<TestEntity>("test", new ClientSearchRequestV4(), token);

            //Assert
            Assert.NotNull(found);
            Assert.Equal(10, found.Entities.Length);
            foreach (var i in Enumerable.Range(5, 10))
            {
                Assert.Contains(found.Entities, f => f.Content.Id == i);
            }
        }

        [Fact]
        public async Task ShouldDenyByTokenAudience()
        {
            //Arrange
            var cl = _searchClient.StartWithProxy(srv => srv.Configure<SearcherOptions>(o =>
            {
                o.Token = new TokenizingOptions
                {
                    SignKey = string.Join(';', Enumerable.Repeat(Guid.NewGuid().ToString("N"), 10))
                };
            }));

            var tokenRequest = new MyLab.Search.SearcherClient.TokenRequestV4()
            {
                Indexes = Array.Empty<IndexSettingsV4>()
            };

            ResponseCodeException actualError = null;

            //Act
            string token = await cl.CreateSearchTokenAsync(tokenRequest);

            try
            {
                await cl.SearchAsync<TestEntity>("test", new ClientSearchRequestV4(), token);
            }
            catch (ResponseCodeException e)
            {
                actualError = e;
            }

            //Assert
            Assert.NotNull(actualError);
            Assert.Equal(HttpStatusCode.Forbidden, actualError.StatusCode);
        }

        [Theory]
        [InlineData("test")]
        [InlineData("*")]
        public async Task ShouldAcceptByTokenAudience(string indexId)
        {
            //Arrange
            var cl = _searchClient.StartWithProxy(srv => srv.Configure<SearcherOptions>(o =>
            {
                o.Token = new TokenizingOptions
                {
                    SignKey = string.Join(';', Enumerable.Repeat(Guid.NewGuid().ToString("N"), 10))
                };
            }));

            var tokenRequest = new MyLab.Search.SearcherClient.TokenRequestV4()
            {
                Indexes = new IndexSettingsV4[]
                {
                    new IndexSettingsV4
                    {
                        Id = indexId
                    }
                }
            };

            //Act
            string token = await cl.CreateSearchTokenAsync(tokenRequest);

            var found = await cl.SearchAsync<TestEntity>("test", new ClientSearchRequestV4(), token);

            //Assert
            Assert.NotNull(found);
            Assert.True(found.Total != 0);
        }
    }
}