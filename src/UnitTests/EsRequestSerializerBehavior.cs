using MyLab.Search.Delegate.Models;
using MyLab.Search.Delegate.Tools;
using Xunit;
using Xunit.Abstractions;

namespace UnitTests
{
    public class EsRequestSerializerBehavior
    {
        private readonly ITestOutputHelper _output;
        private readonly EsSearchRequestSerializer _serializer;

        public EsRequestSerializerBehavior(ITestOutputHelper output)
        {
            _output = output;
            _serializer = new EsSearchRequestSerializer(true);
        }

        [Fact]
        public void ShouldSerializeOffset()
        {
            //Arrange
            var m = new EsSearchModel
            {
                From = 10,
                Size = 20
            };

            //Act
            var stringRes = Serialize(m);

            //Assert
            Assert.Contains("\"from\": 10", stringRes);
            Assert.Contains("\"size\": 20", stringRes);
        }

        [Fact]
        public void ShouldExcludeNullValues()
        {
            //Arrange
            var m = new EsSearchModel
            {
                From = 10
            };

            //Act
            var stringRes = Serialize(m);

            //Assert
            Assert.DoesNotContain("query:", stringRes);
            Assert.DoesNotContain("sort:", stringRes);
        }

        [Fact]
        public void ShouldSerializeFilters()
        {
            //Arrange
            const string filter1 = "{\"term\":{\"x\":{\"value\":\"foo\"}}}";
            const string filter2 = "{\"term\":{\"x\":{\"value\":\"bar\"}}}";
            var m = new EsSearchModel
            {
                From = 10,
                Size = 20,
                Query = new EsSearchQueryModel
                {
                    Bool = new EsSearchQueryBoolModel
                    {
                        Must = new []
                        {
                            filter1,
                            filter2
                        }
                    }
                }
            };

            //Act
            var stringRes = Serialize(m);

            //Assert
            Assert.Contains(
                $"\"must\": [{filter1},{filter2}]",
                stringRes);
        }

        [Fact]
        public void ShouldSerializeSorts()
        {
            //Arrange
            const string sort1 = "{\"foo\":{\"order\":\"asc\"}}";
            const string sort2 = "{\"bar\":{\"order\":\"asc\"}}";
            var m = new EsSearchModel
            {
                From = 10,
                Size = 20,
                Sort = new[]
                {
                    sort1,
                    sort2
                }
            };

            //Act
            var stringRes = Serialize(m);

            //Assert
            Assert.Contains(
                $"\"sort\": [{sort1},{sort2}]",
                stringRes);
        }

        string Serialize(EsSearchModel model)
        {
            var res =_serializer.Serialize(model);

            _output.WriteLine(res);
            return res;
        }
    }
}
