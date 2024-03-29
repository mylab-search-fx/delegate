using System.Web;
using Xunit;

namespace UnitTests
{
    public class HttpUtilityBehavior
    {
        [Theory]
        [InlineData("foo\\", "foo\\\\")]
        [InlineData("foo\"", "foo\\\"")]
        public void ShouldEscapeString(string input, string expected)
        {
            //Arrange


            //Act
           var actual = HttpUtility.JavaScriptStringEncode(input);

            //Assert
            Assert.Equal(expected, actual);
        }
    }
}
