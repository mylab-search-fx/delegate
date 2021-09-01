using System;
using System.Threading.Tasks;
using MyLab.Search.Delegate.Models;
using MyLab.Search.Delegate.Services;
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
            public Task<SearchSort> ProvideAsync(string sortId, string ns)
            {
                throw new NotImplementedException();
            }
        }

        private class TestFilterProvider : IEsFilterProvider
        {
            public Task<SearchFilter> ProvideAsync(string filterId, string ns)
            {
                throw new NotImplementedException();
            }
        }

        private class TestIndexMappingService : IIndexMappingService
        {
            public Task<IndexMapping> GetIndexMappingAsync(string ns)
            {
                return Task.FromResult(new IndexMapping(new[]
                {
                    new IndexMappingProperty("Blocked", "boolean"),
                    new IndexMappingProperty("ChamberId", "keyword"),
                    new IndexMappingProperty("ChamberName", "text"),
                    new IndexMappingProperty("DeletedInEis", "boolean"),
                    new IndexMappingProperty("DeletedInInfonot", "boolean"),
                    new IndexMappingProperty("GivenName", "text"),
                    new IndexMappingProperty("Id", "keyword"),
                    new IndexMappingProperty("LastChangeDt", "date"),
                    new IndexMappingProperty("LastName", "text"),
                    new IndexMappingProperty("NotaryEnabled", "boolean"),
                    new IndexMappingProperty("Type", "keyword"),
                }));
            }
        }
    }
}