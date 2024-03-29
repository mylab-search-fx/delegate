using System;
using System.Linq;
using MyLab.Search.Searcher.QueryTools;
using Xunit;

namespace UnitTests
{
    public class SearchQueryProcessorBehavior
    {
        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   \t\t")]
        public void ShouldCreateEmptyWhenEmptyQuery(string query)
        {
            //Arrange

            //Act
            var q = SearchQueryApplier.Parse(query);

            //Assert
            Assert.Empty(q.Items);
        }

        [Theory]
        [InlineData("a")]
        [InlineData("aa")]
        public void ShouldIgnoreShortWords(string query)
        {
            //Arrange

            //Act
            var q = SearchQueryApplier.Parse(query);

            //Assert
            Assert.Empty(q.Items);
        }

        [Fact]
        public void ShouldDetectTextParam()
        {
            //Arrange
            string query = "foo";

            //Act
            var q = SearchQueryApplier.Parse(query);
            var p = q.Items.FirstOrDefault()
                ?.Expressions.FirstOrDefault() as WorldQueryExpression;
            
            //Assert
            Assert.NotNull(p);
            Assert.Equal("foo", p.Literal);
        }

        [Fact]
        public void ShouldSetRightBoosts()
        {
            //Arrange
            string query = "foo bar";

            //Act
            var q = SearchQueryApplier.Parse(query);

            var boosts = q.Items
                .Select(p => p.Boost)
                .ToArray();

            //Assert
            Assert.Equal(3, q.FullQueryItem.Boost);
            Assert.Equal(2, boosts.Length);
            Assert.Equal(2, boosts[0]);
            Assert.Equal(1, boosts[1]);
        }

        [Fact]
        public void ShouldDetectNumeric()
        {
            //Arrange
            string query = "123";

            //Act
            var q = SearchQueryApplier.Parse(query);

            var numParam = q.Items
                .FirstOrDefault()
                ?.Expressions.FirstOrDefault()
                as NumericQueryExpression;

            //Assert
            Assert.NotNull(numParam);
            Assert.Equal(123, numParam.Value);
        }

        [Theory]
        [InlineData("02/01/2003")]
        [InlineData("01.02.2003")]
        [InlineData("010203")]
        public void ShouldDetectDateTime(string dtString)
        {
            //Arrange
            string query = dtString;
            DateTime expected = new DateTime(2003, 02, 01);

            //Act
            var q = SearchQueryApplier.Parse(query);

            var dtParam = q.Items
                    .FirstOrDefault(p => p.Boost == 1)
                    ?.Expressions.FirstOrDefault()
                as RangeDateQueryExpression;

            //Assert
            Assert.NotNull(dtParam);
            Assert.Equal(expected, dtParam.From);
            Assert.Equal(expected.AddDays(1), dtParam.To);
        }

        [Theory]
        [InlineData("<222", null, 222)]
        [InlineData(">123", 123, null)]
        public void ShouldDetectNumericRange(string query, int? from, int? to)
        {
            //Arrange

            //Act
            var q = SearchQueryApplier.Parse(query);

            var numParam = q.Items
                    .FirstOrDefault()
                    ?.Expressions.FirstOrDefault()
                as RangeNumericQueryExpression;

            //Assert
            Assert.NotNull(numParam);
            Assert.Equal(from, numParam.Greater);
            Assert.Equal(to, numParam.Less);
        }

        [Theory]
        [InlineData("123-222", 123, 222)]
        public void ShouldDetectFullNumericRange(string query, int? from, int? to)
        {
            //Arrange

            //Act
            var q = SearchQueryApplier.Parse(query);

            var numParam = q.Items
                    .FirstOrDefault()
                    ?.Expressions.FirstOrDefault()
                as RangeNumericQueryExpression;

            //Assert
            Assert.NotNull(numParam);
            Assert.Equal(from, numParam.GreaterOrEqual);
            Assert.Equal(to, numParam.LessOrEqual);
        }

        [Theory]
        [InlineData("02/01/2003-02/02/2003")]
        [InlineData("01.02.2003-02.02.2003")]
        [InlineData("010203-020203")]
        public void ShouldDetectDateTimeRange(string dtString)
        {
            //Arrange
            string query = dtString;
            DateTime expectedFrom = new DateTime(2003, 02, 01);
            DateTime expectedTo = new DateTime(2003, 02, 02);

            //Act
            var q = SearchQueryApplier.Parse(query);

            var dtParam = q.Items
                    .FirstOrDefault(p => p.Boost == 1)
                    ?.Expressions.FirstOrDefault()
                as RangeDateQueryExpression;

            //Assert
            Assert.NotNull(dtParam);
            Assert.Equal(expectedFrom, dtParam.From);
            Assert.Equal(expectedTo, dtParam.To);
        }

        [Theory]
        [InlineData("<02/02/2003")]
        [InlineData("<02.02.2003")]
        [InlineData("<020203")]
        public void ShouldDetectDateTimeLess(string dtString)
        {
            //Arrange
            string query = dtString;
            DateTime expectedTo = new DateTime(2003, 02, 02);

            //Act
            var q = SearchQueryApplier.Parse(query);

            var dtParam = q.Items
                    .FirstOrDefault(p => p.Boost == 1)
                    ?.Expressions.FirstOrDefault()
                as RangeDateQueryExpression;

            //Assert
            Assert.NotNull(dtParam);
            Assert.Null(dtParam.From);
            Assert.Equal(expectedTo, dtParam.To);
        }

        [Theory]
        [InlineData(">02/01/2003")]
        [InlineData(">01.02.2003")]
        [InlineData(">010203")]
        public void ShouldDetectDateTimeGreater(string dtString)
        {
            //Arrange
            string query = dtString;
            DateTime expectedFrom = new DateTime(2003, 02, 01);

            //Act
            var q = SearchQueryApplier.Parse(query);

            var dtParam = q.Items
                    .FirstOrDefault(p => p.Boost == 1)
                    ?.Expressions.FirstOrDefault()
                as RangeDateQueryExpression;

            //Assert
            Assert.NotNull(dtParam);
            Assert.Equal(expectedFrom, dtParam.From);
            Assert.Null(dtParam.To);
        }

        [Fact]
        public void ShouldDetectManyDigitsAsTextToo()
        {
            //Arrange
            string query = "94200005";

            //Act
            var q = SearchQueryApplier.Parse(query);
            var t = q.Items.FirstOrDefault()?.Expressions.OfType<WorldQueryExpression>().FirstOrDefault();
            var n = q.Items.FirstOrDefault()?.Expressions.OfType<NumericQueryExpression>().FirstOrDefault();

            //Assert
            Assert.NotNull(t);
            Assert.NotNull(n);
            Assert.Equal("94200005", t.Literal);
            Assert.Equal(94200005, n.Value);
        }
    }
}
