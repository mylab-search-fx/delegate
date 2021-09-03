using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MyLab.Search.Delegate;
using Xunit;

namespace UnitTests
{
    public class DelegateOptionsBehavior
    {
        [Theory]
        [InlineData("must")]
        [InlineData("Must")]
        public void ShouldDeserializeQueryStrategy(string mustStr)
        {
            //Arrange
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new KeyValuePair<string,string>[]
                {
                    new KeyValuePair<string, string>("test:QueryStrategy", mustStr), 
                })
                .Build();

            var srv = new ServiceCollection()
                .Configure<DelegateOptions>(config.GetSection("test"))
                .BuildServiceProvider();

            //Act
            var opt = srv.GetService<IOptions<DelegateOptions>>();

            //Assert
            Assert.Equal(QuerySearchStrategy.Must, opt.Value.QueryStrategy);
        }

        [Fact]
        public void ShouldProvideNamespace()
        {
            //Arrange
            var opts = new DelegateOptions
            {
                Namespaces = new []
                {
                    new DelegateOptions.Namespace
                    {
                        Name = "foo",
                        Index = "foo-index"
                    },
                    new DelegateOptions.Namespace
                    {
                        Name = "bar",
                        Index = "bar-index"
                    },
                }
            };

            //Act
            var foundNs = opts.GetNamespace("foo");

            //Assert
            Assert.Equal("foo-index", foundNs.Index);
        }

        [Fact]
        public void ShouldThrowIfNsNotFound()
        {
            //Arrange
            var opts = new DelegateOptions
            {
                Namespaces = new[]
                {
                    new DelegateOptions.Namespace
                    {
                        Name = "bar",
                        Index = "bar-index"
                    },
                }
            };

            //Act & Assert
            Assert.Throws<InvalidOperationException>(() => opts.GetNamespace("foo"));
        }

        [Theory]
        [InlineData(null, null, "foo")]
        [InlineData("prefix_", null, "prefix_foo")]
        [InlineData("prefix_", "_postfix", "prefix_foo_postfix")]
        [InlineData(null, "_postfix", "foo_postfix")]
        public void ShouldProvideIndexName(string prefix, string postfix, string expectedResult)
        {
            //Arrange
            var opts = new DelegateOptions
            {
                IndexNamePrefix = prefix,
                IndexNamePostfix = postfix,
                Namespaces = new[]
                {
                    new DelegateOptions.Namespace
                    {
                        Name = "bar",
                        Index = "foo"
                    }
                }
            };

            //Act
            var indexName = opts.GetIndexName("bar");

            //Assert
            Assert.Equal(expectedResult, indexName);
        }

        [Fact]
        public void ShouldThrowWhenGetIndexNameAndNsNotFound()
        {
            //Arrange
            var opts = new DelegateOptions
            {
                Namespaces = new[]
                {
                    new DelegateOptions.Namespace
                    {
                        Name = "bar",
                        Index = "bar-index"
                    },
                }
            };

            //Act & Assert
            Assert.Throws<InvalidOperationException>(() => opts.GetIndexName("foo"));
        }
    }
}