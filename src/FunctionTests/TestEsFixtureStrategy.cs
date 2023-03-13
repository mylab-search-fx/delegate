using System;
using Elasticsearch.Net;
using MyLab.Search.EsTest;

namespace FunctionTests
{
    public class TestEsFixtureStrategy : EsFixtureStrategy
    {
        public override IConnectionPool ProvideConnection()
        {
            return new SingleNodeConnectionPool(new Uri("http://localhost:9200"));
        }
    }
}