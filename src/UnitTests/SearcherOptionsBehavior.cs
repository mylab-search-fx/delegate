using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MyLab.Search.Searcher;
using MyLab.Search.Searcher.Options;
using Xunit;

namespace UnitTests
{
    public class SearcherOptionsBehavior
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
                .Configure<SearcherOptions>(config.GetSection("test"))
                .BuildServiceProvider();

            //Act
            var opt = srv.GetService<IOptions<SearcherOptions>>();

            //Assert
            Assert.Equal(QuerySearchStrategy.Must, opt.Value.QueryStrategy);
        }

        [Fact]
        public void ShouldProvideIndexOpts()
        {
            //Arrange
            var opts = new SearcherOptions
            {
                Indexes = new []
                {
                    new IdxOptions
                    {
                        Id = "foo",
                        EsIndex = "foo-index"
                    },
                    new IdxOptions
                    {
                        Id = "bar",
                        EsIndex = "bar-index"
                    },
                }
            };

            //Act
            var foundNs = opts.GetIndexOptions("foo");

            //Assert
            Assert.Equal("foo-index", foundNs.EsIndex);
        }

        [Fact]
        public void ShouldPassIfNsNotFound()
        {
            //Arrange
            var opts = new SearcherOptions
            {
                Indexes = new[]
                {
                    new IdxOptions
                    {
                        Id = "bar",
                        EsIndex = "bar-index"
                    },
                }
            };

            //Act & Assert
            opts.GetIndexOptions("foo");
        }

        [Theory]
        [InlineData("foo", null, null, "foo")]
        [InlineData("foo", "prefix_", null, "prefix_foo")]
        [InlineData("foo", "prefix_", "_postfix", "prefix_foo_postfix")]
        [InlineData("foo", null, "_postfix", "foo_postfix")]
        [InlineData(null, null, null, "bar")]
        [InlineData(null, "prefix_", null, "prefix_bar")]
        [InlineData(null, "prefix_", "_postfix", "prefix_bar_postfix")]
        [InlineData(null, null, "_postfix", "bar_postfix")]
        public void ShouldCreateEsIndexName(string esIndexName, string prefix, string postfix, string expectedResult)
        {
            //Arrange
            var opts = new SearcherOptions
            {
                EsIndexNamePrefix = prefix,
                EsIndexNamePostfix = postfix,
                Indexes = new[]
                {
                    new IdxOptions
                    {
                        Id = "bar",
                        EsIndex = esIndexName
                    }
                }
            };

            //Act
            var indexName = opts.CreateEsIndexName("bar");

            //Assert
            Assert.Equal(expectedResult, indexName);
        }

        [Fact]
        public void ShouldPassWhenGetIndexNameAndNsNotFound()
        {
            //Arrange
            var opts = new SearcherOptions
            {
                Indexes = new[]
                {
                    new IdxOptions
                    {
                        Id = "bar",
                        EsIndex = "bar-index"
                    },
                }
            };

            //Act & Assert
            opts.CreateEsIndexName("foo");
        }
    }
}