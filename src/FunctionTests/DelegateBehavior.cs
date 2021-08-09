using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MyLab.Search.Delegate;
using Xunit;

namespace FunctionTests
{
    public partial class DelegateBehavior
    {
        [Fact]
        public async Task ShouldPerformSimpleSearch()
        {
            //Arrange
            var cl = _client.StartWithProxy();

            //Act
            var found = await cl.SearchAsync();

            //Assert
            Assert.NotNull(found);
            Assert.Equal(10, found.Length);
        }

        [Fact]
        public async Task ShouldUseDefaultLimit()
        {
            //Arrange
            var cl = _client.StartWithProxy(srv => srv
                .Configure<DelegateOptions>(o =>
                {
                    o.DefaultLimit = 5;
                }));

            //Act
            var found = await cl.SearchAsync();

            //Assert
            Assert.NotNull(found);
            Assert.Equal(5, found.Length);
        }

        [Fact]
        public async Task ShouldUseSpecifiedLimit()
        {
            //Arrange
            var cl = _client.StartWithProxy();

            //Act
            var found = await cl.SearchAsync(limit: 3);

            //Assert
            Assert.NotNull(found);
            Assert.Equal(3, found.Length);
        }

        [Fact]
        public async Task ShouldUseDefaultSort()
        {
            //Arrange
            var cl = _client.StartWithProxy(srv => srv
                .Configure<DelegateOptions>(o =>
                {
                    o.DefaultSort = "revert";
                })
            );

            //Act
            var found = await cl.SearchAsync();

            //Assert
            Assert.NotNull(found);
            Assert.Equal(20, found[0].Id);
        }

        [Fact]
        public async Task ShouldUseSpecifiedSort()
        {
            //Arrange
            var cl = _client.StartWithProxy();

            //Act
            var found = await cl.SearchAsync(sort: "revert");

            //Assert
            Assert.NotNull(found);
            Assert.Equal(20, found[0].Id);
        }

        [Fact]
        public async Task ShouldUseSpecifiedOffset()
        {
            //Arrange
            var cl = _client.StartWithProxy();

            //Act
            var found = await cl.SearchAsync(sort: "revert", offset:1);

            //Assert
            Assert.NotNull(found);
            Assert.Equal(19, found[0].Id);
        }

        [Fact]
        public async Task ShouldUseDefaultFilter()
        {
            //Arrange
            var cl = _client.StartWithProxy(srv => srv
                .Configure<DelegateOptions>(o =>
                {
                    o.DefaultFilter = "from5to15";
                })
            );

            //Act
            var found = await cl.SearchAsync();

            //Assert
            Assert.NotNull(found);
            foreach (var i in Enumerable.Range(5, 10))
            {
                Assert.Contains(found, f => f.Id == i);
            }
        }

        [Fact]
        public async Task ShouldUseSpecifiedFilter()
        {
            //Arrange
            var cl = _client.StartWithProxy();

            //Act
            var found = await cl.SearchAsync(filter: "from5to15");

            //Assert
            Assert.NotNull(found);
            foreach (var i in Enumerable.Range(5, 10))
            {
                Assert.Contains(found, f => f.Id == i);
            }
        }

        [Fact]
        public async Task ShouldUseFullRequestWithoutQuery()
        {
            //Arrange
            var cl = _client.StartWithProxy();

            //Act
            var found = await cl.SearchAsync(
                filter: "from5to15",
                sort: "revert",
                offset: 1,
                limit: 1);

            //Assert
            Assert.NotNull(found);
            Assert.Single(found);
            Assert.Equal(13, found[0].Id);
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
