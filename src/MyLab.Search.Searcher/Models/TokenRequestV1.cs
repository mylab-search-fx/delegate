using System;
using System.Collections.Generic;
using Newtonsoft.Json;

#if IS_CLIENT
namespace MyLab.Search.Searcher.Client
#else
namespace MyLab.Search.Searcher.Models
#endif

{
    [Obsolete]
    public class TokenRequestV1
    {
        [JsonProperty("namespaces")]
        public NamespaceSettingsMapV1 Namespaces { get; set; }
    }

    [Obsolete]
    public class NamespaceSettingsMapV1 : Dictionary<string, NamespaceSettingsV1>
    {
    }

    [Obsolete]
    public class NamespaceSettingsV1
    {
        [JsonProperty("filters")]
        public FiltersCall Filters { get; set; } 
    }

}