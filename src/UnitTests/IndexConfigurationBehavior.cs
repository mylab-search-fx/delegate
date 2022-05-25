using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MyLab.Search.Searcher;
using MyLab.Search.Searcher.Options;
using Xunit;

namespace UnitTests
{
    public class IndexConfigurationBehavior
    {
        [Theory]
        [InlineData("appsettings-tabs.json")]
        [InlineData("appsettings-spaces.json")]
        public void ShouldLoadConfiguration(string filename)
        {
            //Arrange
            var config = new ConfigurationBuilder()
                .AddJsonFile(Path.Combine("files", filename))
                .Build();

            var srv = new ServiceCollection()
                .Configure<SearcherOptions>(config.GetSection("Searcher"))
                .BuildServiceProvider();

            //Act
            var opts = srv.GetService<IOptions<SearcherOptions>>();

            //Assert
            Assert.NotNull(opts);
            Assert.NotNull(opts.Value);
            Assert.NotNull(opts.Value.Indexes);
            Assert.Contains(opts.Value.Indexes, idx => idx.Id == "addressees");
        }
    }
}
