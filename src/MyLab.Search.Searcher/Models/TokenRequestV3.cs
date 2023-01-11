using Newtonsoft.Json;

#if IS_CLIENT
namespace MyLab.Search.SearcherClient
#else
namespace MyLab.Search.Searcher.Models
#endif

{
    public class TokenRequestV3
    {
        [JsonProperty("namespaces")]
        public NamespaceSettingsV3[] Namespaces { get; set; }
    }

    public class NamespaceSettingsV3
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("filters")]
        public FilterRef[] Filters { get; set; }
    }


}