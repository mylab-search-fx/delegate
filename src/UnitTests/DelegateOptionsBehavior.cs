using System;
using MyLab.Search.Delegate;
using Xunit;

namespace UnitTests
{
    public class DelegateOptionsBehavior
    {
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
        public void ShouldThrowWhenGetindexNameAndNsNotFound()
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