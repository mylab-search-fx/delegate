using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MyLab.Search.Searcher;
using Xunit;

namespace UnitTests
{
    public class NamespaceConfigurationBehavior
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
            Assert.NotNull(opts.Value.Namespaces);
            Assert.Contains(opts.Value.Namespaces, ns => ns.Name == "addressees");
        }
    }
}
