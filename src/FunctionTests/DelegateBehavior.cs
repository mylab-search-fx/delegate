using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MyLab.Search.Delegate;
using MyLab.Search.Delegate.Services;
using MyLab.Search.EsAdapter.SearchEngine;
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
    }
}
