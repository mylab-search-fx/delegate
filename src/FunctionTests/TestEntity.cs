using Nest;

namespace FunctionTests
{
    [ElasticsearchType(IdProperty = nameof(Id))]
    public class TestEntity
    {
        [Number(Name = "Id")]
        public int Id { get; set; }
        [Text(Name = "Value")]
        public string Value { get; set; }
    }
}