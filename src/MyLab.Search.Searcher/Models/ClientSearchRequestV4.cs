using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

#if IS_CLIENT
namespace MyLab.Search.Searcher.Client
#else
namespace MyLab.Search.Searcher.Models
#endif
{
    public class ClientSearchRequestV4
    {
        [JsonProperty("query")]
        public string Query { get; set; }
        [JsonProperty("filters")]
        public FilterRef[] Filters { get; set; }
        [JsonProperty("sort")]
        public SortingRef Sort { get; set; }
        [JsonProperty("offset")]
        public int Offset { get; set; }
        [JsonProperty("limit")]
        public int Limit { get; set; }
        [JsonProperty("queryMode")]
        [JsonConverter(typeof(StringEnumConverter))]
        public QuerySearchStrategy QuerySearchStrategy { get; set; }
    }
}