using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyLab.Search.Searcher;
using MyLab.Search.Searcher.Models;
using MyLab.Search.Searcher.Options;
using MyLab.Search.Searcher.QueryTools;
using MyLab.Search.Searcher.Services;
using MyLab.Search.Searcher.Tools;
using Nest;
using Xunit;
using Xunit.Abstractions;

namespace UnitTests
{
    public partial class RequestBuilderBehavior
    {
        private readonly ITestOutputHelper _output;

        public RequestBuilderBehavior(ITestOutputHelper output)
        {
            _output = output;
        }

        private class TestSortProvider : IEsSortProvider
        {
            public Task<ISort> ProvideAsync(string sortId, string ns, IEnumerable<KeyValuePair<string, string>> args = null)
            {
                throw new NotImplementedException();
            }

            public Task<ISort> ProvideDefaultAsync(string ns)
            {
                return Task.FromResult<ISort>(null);
            }
        }

        private class TestFilterProvider : IEsFilterProvider
        {
            public Task<QueryContainer> ProvideAsync(string filterId, string ns, IEnumerable<KeyValuePair<string,string>> args = null)
            {
                throw new NotImplementedException();
            }
        }

        private class TestIndexMappingService : IIndexMappingService
        {
            public Task<TypeMapping> GetIndexMappingAsync(string ns)
            {
                var props = new List<KeyValuePair<PropertyName, IProperty>>
                {
                    Prop<BooleanProperty>("Blocked"),
                    Prop<BooleanProperty>("NotaryEnabled"),
                    Prop<BooleanProperty>("DeletedInEis"),
                    Prop<BooleanProperty>("DeletedInInfonot"),

                    Prop<KeywordProperty>("ChamberId"),
                    Prop<KeywordProperty>("Type"),
                    Prop<KeywordProperty>("Id"),
                    
                    Prop<TextProperty>("ChamberName"),
                    Prop<TextProperty>("GivenName"),
                    Prop<TextProperty>("LastName"),
                };


                return Task.FromResult(new TypeMapping
                {
                    Properties = new Properties(
                            new Dictionary<PropertyName, IProperty>(props)
                    )
                });

                KeyValuePair<PropertyName, IProperty> Prop<TProp>(string name) 
                    where TProp : IProperty, new()
                {
                    var propName = new PropertyName(name);
                    IProperty prop = new TProp
                    {
                        Name = propName
                    };

                    return new KeyValuePair<PropertyName, IProperty>(propName, prop);
                }
            }
        }
        
        async Task<SearchRequestPlan> BuildRequestByQueryAsync(string text)
        {
            var opt = new SearcherOptions
            {
                QueryStrategy = QuerySearchStrategy.Must,
                Indexes = new[]
                {
                    new IdxOptions
                    {
                        Id = "test"
                    }
                }
            };

            var reqBuilder = new EsRequestBuilder(opt,
                new TestSortProvider(),
                new TestFilterProvider(),
                new TestIndexMappingService());

            var sReq = new ClientSearchRequestV4
            {
                Query = text
            };

            var plan = reqBuilder.BuildPlan(sReq, "test", null);
            var esReq = await reqBuilder.BuildRequestAsync(plan, "test");

            var str = await EsSerializer.Instance.SerializeAsync(esReq);

            _output.WriteLine(str);

            return plan;
        }
    }
}