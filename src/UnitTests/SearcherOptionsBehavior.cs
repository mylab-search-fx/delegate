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
        public void ShouldThrowIfNsNotFound()
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
            Assert.Throws<IndexOptionsNotFoundException>(() => opts.GetIndexOptions("foo"));
        }

        [Theory]
        [InlineData(null, null, "foo")]
        [InlineData("prefix_", null, "prefix_foo")]
        [InlineData("prefix_", "_postfix", "prefix_foo_postfix")]
        [InlineData(null, "_postfix", "foo_postfix")]
        public void ShouldCreateEsIndexName(string prefix, string postfix, string expectedResult)
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
                        EsIndex = "foo"
                    }
                }
            };

            //Act
            var indexName = opts.CreateEsIndexName("bar");

            //Assert
            Assert.Equal(expectedResult, indexName);
        }

        [Fact]
        public void ShouldThrowWhenGetIndexNameAndNsNotFound()
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
            Assert.Throws<IndexOptionsNotFoundException>(() => opts.CreateEsIndexName("foo"));
        }
    }
}