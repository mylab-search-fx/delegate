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
        public async Task ShouldUseLimit()
        {
            //Arrange
            var cl = _client.StartWithProxy();

            //Act
            var found = await cl.SearchAsync(limit: 3);

            //Assert
            Assert.NotNull(found);
            Assert.Equal(3, found.Length);
        }
    }
}
