using System.Collections.Generic;
using System.IO;
using System.Text;
using Elasticsearch.Net;
using MyLab.Search.Searcher.Tools;
using Nest;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace UnitTests
{
    public class EsModelSerializationBehavior
    {
        private readonly ITestOutputHelper _output;

        private readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
            {NullValueHandling = NullValueHandling.Ignore};

    public EsModelSerializationBehavior(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void ShouldBoolSerialize()
        {
            //Arrange
            var b = new BoolQuery
            {
                MinimumShouldMatch = 2,
                Should = new QueryContainer[]
                {
                    new TermQuery
                    {
                        Field = "Fiela",
                        Name = "PropName",
                        Value = "Value"
                    },
                    new MatchQuery
                    {
                        Field = "Field",
                        Query = "query"
                    }
                },
                Filter = new List<QueryContainer>()
                {
                    new MatchQuery
                    {
                        Field = "Field",
                        Query = "query"
                    }
                }
            };

            //Act
            var str = EsSerializer.Instance.SerializeToString(b);
            _output.WriteLine(str);

            //Assert

        }

        [Fact]
        public void ShouldDeserializeSort()
        {
            //Arrange
            var str = "{\"Id\":{\"order\":\"desc\"}}";
            
            //Act
            using var strm = new MemoryStream(Encoding.UTF8.GetBytes(str));
            var sort = EsSerializer.Instance.Deserialize<ISort>(strm) as FieldSort;

            //Assert
            Assert.NotNull(sort);
            Assert.Equal("Id", sort.Field.Name);
            Assert.Equal(SortOrder.Descending, sort.Order);
        }

        [Fact]
        public void ShouldDeserializeFilter()
        {
            //Arrange
            var str = "{\"range\":{\"Id\":{\"gte\":2,\"lt\":5}}}";

            //Act
            using var strm = new MemoryStream(Encoding.UTF8.GetBytes(str));
            var queryContainer = (IQueryContainer)EsSerializer.Instance.Deserialize<QueryContainer>(strm);
            var rangeQuery = queryContainer.Range as LongRangeQuery;

            //Assert
            Assert.NotNull(rangeQuery);
            Assert.Equal(2, rangeQuery.GreaterThanOrEqualTo);
            Assert.Equal(5, rangeQuery.LessThan);
            Assert.Equal("Id", rangeQuery.Field);
        }
    }
}
