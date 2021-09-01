using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MyLab.Search.Delegate;
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
                .Configure<DelegateOptions>(config.GetSection("Delegate"))
                .BuildServiceProvider();

            //Act
            var opts = srv.GetService<IOptions<DelegateOptions>>();

            //Assert
            Assert.NotNull(opts);
            Assert.NotNull(opts.Value);
            Assert.NotNull(opts.Value.Namespaces);
            Assert.Contains(opts.Value.Namespaces, ns => ns.Name == "addressees");
        }
    }
}
