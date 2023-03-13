using System;
using Nest;

namespace FunctionTests
{
    [ElasticsearchType(IdProperty = nameof(Id))]
    public class TestEntity
    {
        [Number(Name = "Id")]
        public int Id { get; set; }

        [Keyword(Name = "Keyword")]
        public string Keyword { get; set; }

        [Text(Name = "Value")]
        public string Value { get; set; }

        [Date(Name = "Date")]
        public DateTime Date { get; set; }
    }

    [ElasticsearchType(IdProperty = nameof(Id))]
    public class TestEntityWithNotIndexedField : TestEntity
    {
        [Text(Name = "NotIndexed",  Index=false)]
        public string NotIndexed { get; set; }
    }
}