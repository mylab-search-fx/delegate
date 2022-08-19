using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyLab.Search.Searcher.Services;
using Nest;
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
    }
}