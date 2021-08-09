using System;
using Elasticsearch.Net;
using MyLab.Search.EsTest;

namespace FunctionTests
{
    public class TestConnectionProvider : IConnectionProvider
    {
        public IConnectionPool Provide()
        {
            return new SingleNodeConnectionPool(new Uri("http://localhost:9200"));
        }
    }
}