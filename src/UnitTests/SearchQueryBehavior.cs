using System;
using System.Linq;
using MyLab.Search.Delegate.QueryStuff;
using Xunit;

namespace UnitTests
{
    public class SearchQueryBehavior
    {
        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   \t\t")]
        public void ShouldCreateEmptyWhenEmptyQuery(string query)
        {
            //Arrange

            //Act
            var q = SearchQuery.Parse(query);

            //Assert
            Assert.Empty(q.DateTimeParams);
            Assert.Empty(q.NumericParams);
            Assert.Empty(q.TextParams);
        }

        [Theory]
        [InlineData("a")]
        [InlineData("aa")]
        public void ShouldIgnoreShortWords(string query)
        {
            //Arrange

            //Act
            var q = SearchQuery.Parse(query);

            //Assert
            Assert.Empty(q.DateTimeParams);
            Assert.Empty(q.NumericParams);
            Assert.Empty(q.TextParams);
        }

        [Fact]
        public void ShouldDetectTextParam()
        {
            //Arrange
            string query = "foo";

            //Act
            var q = SearchQuery.Parse(query);
            var p = q.TextParams.FirstOrDefault() as TextQueryParameter;
            
            //Assert
            Assert.Single(q.TextParams);
            Assert.Equal("foo", p.Value);
        }

        [Fact]
        public void ShouldSetRightRanks()
        {
            //Arrange
            string query = "foo bar";

            //Act
            var q = SearchQuery.Parse(query);

            var ranks = q.TextParams
                .Select(p => p.Rank)
                .ToArray();

            //Assert
            Assert.Equal(2, ranks.Length);
            Assert.Equal(2, ranks[0]);
            Assert.Equal(1, ranks[1]);
        }

        [Fact]
        public void ShouldDetectNumeric()
        {
            //Arrange
            string query = "123";

            //Act
            var q = SearchQuery.Parse(query);

            var numParam = q.NumericParams
                .FirstOrDefault()
                as NumericQueryParameter;

            //Assert
            Assert.NotNull(numParam);
            Assert.Equal(123, numParam.Value);
        }

        [Theory]
        [InlineData("02/01/2003")]
        [InlineData("01.02.2003")]
        public void ShouldDetectDateTime(string dtString)
        {
            //Arrange
            string query = dtString;
            DateTime expected = new DateTime(2003, 02, 01);

            //Act
            var q = SearchQuery.Parse(query);

            var dtParam = q.DateTimeParams
                    .FirstOrDefault(p => p.Rank == 1)
                as DateTimeQueryParameter;

            //Assert
            Assert.NotNull(dtParam);
            Assert.Equal(expected, dtParam.Value);
        }

        [Theory]
        [InlineData("123-222", 123, 222)]
        [InlineData("<222", null, 222)]
        [InlineData(">123", 123, null)]
        public void ShouldDetectNumericRange(string query, int? from, int? to)
        {
            //Arrange

            //Act
            var q = SearchQuery.Parse(query);

            var numParam = q.NumericParams
                    .FirstOrDefault()
                as NumericRangeQueryParameter;

            //Assert
            Assert.NotNull(numParam);
            Assert.Equal(from, numParam.From);
            Assert.Equal(to, numParam.To);
        }

        [Theory]
        [InlineData("02/01/2003-02/02/2003")]
        [InlineData("01.02.2003-02.02.2003")]
        public void ShouldDetectDateTimeRange(string dtString)
        {
            //Arrange
            string query = dtString;
            DateTime expectedFrom = new DateTime(2003, 02, 01);
            DateTime expectedTo = new DateTime(2003, 02, 02);

            //Act
            var q = SearchQuery.Parse(query);

            var dtParam = q.DateTimeParams
                    .FirstOrDefault(p => p.Rank == 1)
                as DateTimeRangeQueryParameter;

            //Assert
            Assert.NotNull(dtParam);
            Assert.Equal(expectedFrom, dtParam.From);
            Assert.Equal(expectedTo, dtParam.To);
        }

        [Theory]
        [InlineData("<02/02/2003")]
        [InlineData("<02.02.2003")]
        public void ShouldDetectDateTimeLess(string dtString)
        {
            //Arrange
            string query = dtString;
            DateTime expectedTo = new DateTime(2003, 02, 02);

            //Act
            var q = SearchQuery.Parse(query);

            var dtParam = q.DateTimeParams
                    .FirstOrDefault(p => p.Rank == 1)
                as DateTimeRangeQueryParameter;

            //Assert
            Assert.NotNull(dtParam);
            Assert.Null(dtParam.From);
            Assert.Equal(expectedTo, dtParam.To);
        }

        [Theory]
        [InlineData(">02/01/2003")]
        [InlineData(">01.02.2003")]
        public void ShouldDetectDateTimeGreater(string dtString)
        {
            //Arrange
            string query = dtString;
            DateTime expectedFrom = new DateTime(2003, 02, 01);

            //Act
            var q = SearchQuery.Parse(query);

            var dtParam = q.DateTimeParams
                    .FirstOrDefault(p => p.Rank == 1)
                as DateTimeRangeQueryParameter;

            //Assert
            Assert.NotNull(dtParam);
            Assert.Equal(expectedFrom, dtParam.From);
            Assert.Null(dtParam.To);
        }
    }
}
