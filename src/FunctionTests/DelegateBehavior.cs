using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MyLab.Search.Delegate;
using MyLab.Search.Delegate.Models;
using Newtonsoft.Json;
using Xunit;

namespace FunctionTests
{
    public partial class DelegateBehavior
    {
        [Fact]
        public async Task ShouldProvideTotalCount()
        {
            //Arrange
            var cl = _searchClient.StartWithProxy();

            //Act
            var found = await cl.SearchAsync<TestEntity>("test", limit: 1);

            //Assert
            Assert.Single(found.Entities);
            Assert.Equal(20, found.Total);
        }

        [Fact]
        public async Task ShouldProvideDebugInfo()
        {
            //Arrange
            var cl = _searchClient.StartWithProxy(srv => srv
                .Configure<DelegateOptions>(o =>
                {
                    o.Debug = true;
                }));

            //Act
            var found = await cl.SearchAsync<TestEntity>("test", limit: 1);
            var foundExplanation = found?.Entities?.First()?.Explanation;

            //Assert
            Assert.NotNull(foundExplanation);
            Assert.NotNull(found.EsRequest);
        }

        [Fact]
        public async Task ShouldReturn500WhenEsRequestError()
        {
            //Arrange
            var cl = _searchClient.Start();

            //Act
            var resp = await cl.Call(s => s.SearchAsync<TestEntity>("test", null, "bad", null, 0, 0, null));

            //Assert
            Assert.Equal(HttpStatusCode.InternalServerError, resp.StatusCode);
        }

        [Fact]
        public async Task ShouldPerformSimpleSearch()
        {
            //Arrange
            var cl = _searchClient.StartWithProxy();

            //Act
            var found = await cl.SearchAsync<TestEntity>("test");

            //Assert
            Assert.NotNull(found);
            Assert.Equal(10, found.Entities.Length);
        }

        [Fact]
        public async Task ShouldUseDefaultLimit()
        {
            //Arrange
            var cl = _searchClient.StartWithProxy(srv => srv
                .Configure<DelegateOptions>(o =>
                {
                    o.Namespaces.First(n => n.Name == "test").DefaultLimit = 5;
                }));

            //Act
            var found = await cl.SearchAsync<TestEntity>("test");

            //Assert
            Assert.NotNull(found);
            Assert.Equal(5, found.Entities.Length);
        }

        [Fact]
        public async Task ShouldUseSpecifiedLimit()
        {
            //Arrange
            var cl = _searchClient.StartWithProxy();

            //Act
            var found = await cl.SearchAsync<TestEntity>("test", limit: 3);

            //Assert
            Assert.NotNull(found);
            Assert.Equal(3, found.Entities.Length);
        }

        [Fact]
        public async Task ShouldUseDefaultSort()
        {
            //Arrange
            var cl = _searchClient.StartWithProxy(srv => srv
                .Configure<DelegateOptions>(o =>
                {
                    o.Namespaces.First(n => n.Name == "test").DefaultSort = "revert";
                }));

            //Act
            var found = await cl.SearchAsync<TestEntity>("test");

            //Assert
            Assert.NotNull(found);
            Assert.Equal(20, found.Entities[0].Content.Id);
        }

        [Fact]
        public async Task ShouldUseSpecifiedSort()
        {
            //Arrange
            var cl = _searchClient.StartWithProxy();

            //Act
            var found = await cl.SearchAsync<TestEntity>("test", sort: "revert");

            //Assert
            Assert.NotNull(found);
            Assert.Equal(20, found.Entities[0].Content.Id);
        }

        [Fact]
        public async Task ShouldUseSpecifiedOffset()
        {
            //Arrange
            var cl = _searchClient.StartWithProxy();

            //Act
            var found = await cl.SearchAsync<TestEntity>("test", sort: "revert", offset:1);

            //Assert
            Assert.NotNull(found);
            Assert.Equal(19, found.Entities[0].Content.Id);
        }

        [Fact]
        public async Task ShouldUseDefaultFilter()
        {
            //Arrange
            var cl = _searchClient.StartWithProxy(srv => srv
                .Configure<DelegateOptions>(o =>
                {
                    o.Namespaces.First(n => n.Name == "test").DefaultFilter = "from5to15";
                })
            );

            //Act
            var found = await cl.SearchAsync<TestEntity>("test");

            //Assert
            Assert.NotNull(found);
            Assert.Equal(10, found.Entities.Length);
            foreach (var i in Enumerable.Range(5, 10))
            {
                Assert.Contains(found.Entities, f => f.Content.Id == i);
            }
        }

        [Fact]
        public async Task ShouldUseSpecifiedFilter()
        {
            //Arrange
            var cl = _searchClient.StartWithProxy();

            //Act
            var found = await cl.SearchAsync<TestEntity>("test", filter: "from5to15");

            //Assert
            Assert.NotNull(found);
            Assert.Equal(10, found.Entities.Length);
            foreach (var i in Enumerable.Range(5, 10))
            {
                Assert.Contains(found.Entities, f => f.Content.Id == i);
            }
        }

        [Fact]
        public async Task ShouldUseFullRequestWithoutQuery()
        {
            //Arrange
            var cl = _searchClient.StartWithProxy();

            //Act
            var found = await cl.SearchAsync<TestEntity>(
                "test",
                filter: "from5to15",
                sort: "revert",
                offset: 1,
                limit: 1);

            //Assert
            Assert.NotNull(found);
            Assert.Single(found.Entities);
            Assert.Equal(13, found.Entities[0].Content.Id);
        }

        [Fact]
        public async Task ShouldUseFilterFromNamespace()
        {
            //Arrange
            var cl = _searchClient.StartWithProxy();

            //Act
            var found = await cl.SearchAsync<TestEntity>("test", filter: "from2to5");

            //Assert
            Assert.NotNull(found);
            Assert.Equal(3, found.Entities.Length);
            foreach (var i in Enumerable.Range(2, 3))
            {
                Assert.Contains(found.Entities, f => f.Content.Id == i);
            }
        }

        [Fact]
        public async Task ShouldUseFilterFromToken()
        {
            //Arrange
            var cl = _searchClient.StartWithProxy(srv => srv.Configure<DelegateOptions>(o =>
            {
                o.Token = new DelegateOptions.Tokenizing
                {
                    SignKey = string.Join(';', Enumerable.Repeat(Guid.NewGuid().ToString("N"), 10))
                };
            }));

            var tokenRequest = new MyLab.Search.Delegate.Client.TokenRequest
            {
                Namespaces = new MyLab.Search.Delegate.Client.NamespaceSettingsMap
                {
                    {
                        "test",
                        new MyLab.Search.Delegate.Client.NamespaceSettings
                        {
                            Filters = new MyLab.Search.Delegate.Client.FiltersCall
                            {
                                {
                                    "paramFilter", new MyLab.Search.Delegate.Client.FilterArgs
                                    {
                                        {"from", "6"},
                                        {"to", "8"}
                                    }
                                }
                            }
                        }
                    }
                }
            };

            //Act
            string token = await cl.CreateSearchTokenAsync(tokenRequest);

            var found = await cl.SearchAsync<TestEntity>("test", searchToken: token);

            //Assert
            Assert.NotNull(found);
            Assert.Equal(2, found.Entities.Length);
            foreach (var i in Enumerable.Range(6, 2))
            {
                Assert.Contains(found.Entities, f => f.Content.Id == i);
            }
        }

        private IEnumerable<TestEntity> CreateTestEntities()
        {
            return Enumerable
                .Range(1, 20)
                .Select(i => new TestEntity
                {
                    Id = i,
                    Value = "val_" + i
                });
        }
    }
}
