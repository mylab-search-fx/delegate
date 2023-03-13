using System.Collections.Generic;
using MyLab.Search.Searcher.Tools;
using Nest;
using Xunit;

namespace UnitTests
{
    public class MappingPropertyToolsBehavior
    {
        [Theory]
        [MemberData(nameof(GetIndexingCases))]
        public void ShouldDetectIndexing(IProperty property, bool expected)
        {
            //Arrange


            //Act
            var isIndexed = MappingPropertyTools.IsIndexed(property);

            //Assert
            Assert.Equal(expected, isIndexed);
        }

        public static IEnumerable<object[]> GetIndexingCases()
        {
            return new object[][]
            {
                new object[] { new TextProperty{ Name = "true", Index = true }, true },
                new object[] { new TextProperty{ Name = "false", Index = false }, false },
                new object[] { new TextProperty{ Name = "null", Index = null }, true },
                new object[] { new PercolatorProperty(), false}
            };
        }
    }
}
