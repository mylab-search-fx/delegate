using System.Threading.Tasks;
using MyLab.Search.Delegate;
using MyLab.Search.Delegate.Models;
using MyLab.Search.Delegate.Services;
using Newtonsoft.Json;
using Xunit;

namespace UnitTests
{
    public partial class RequestBuilderBehavior
    {
        [Theory]
        [InlineData("firstname middlename lastname 123")]
        [InlineData("Супер Иванович Администратор")]
        [InlineData("Проверяющий Тест")]
        public async Task ShouldBuildRequestByQuery(string query)
        {
            //Arrange
            var opt = new DelegateOptions
            {
                Namespaces = new []
                {
                    new DelegateOptions.Namespace
                    {
                        Name = "test"
                    } 
                }
            };
            
            var reqBuilder = new EsRequestBuilder(opt,
                new TestSortProvider(), 
                new TestFilterProvider(), 
                new TestIndexMappingService()); 

            var sReq = new SearchRequest
            {
                Query = query
            };

            //Act
            var esReq = await reqBuilder.BuildAsync(sReq, "test", null);

            _output.WriteLine(JsonConvert.SerializeObject(esReq.Model, Formatting.Indented, 
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                }));

            //Assert

        }
    }
}
