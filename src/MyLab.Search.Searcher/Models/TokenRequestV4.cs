using Newtonsoft.Json;

#if IS_CLIENT
namespace MyLab.Search.SearcherClient
#else
namespace MyLab.Search.Searcher.Models
#endif

{
    public class TokenRequestV4
    {
        [JsonProperty("indexes")]
        public IndexSettingsV4[] Indexes { get; set; }
    }

    public class IndexSettingsV4
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("filters")]
        public FilterRef[] Filters { get; set; }
    }
}