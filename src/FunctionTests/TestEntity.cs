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
}