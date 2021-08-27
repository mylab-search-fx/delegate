using System.Collections.Generic;
using Newtonsoft.Json;

#if IS_CLIENT
namespace MyLab.Search.Delegate.Client
#else
namespace MyLab.Search.Delegate.Models
#endif

{
    public class TokenRequest
    {
        [JsonProperty("namespaces")]
        public NamespaceSettingsMap Namespaces { get; set; }
    }

    public class NamespaceSettingsMap : Dictionary<string, NamespaceSettings>
    {
    }

    public class NamespaceSettings
    {
        [JsonProperty("filters")]
        public FiltersCall Filters { get; set; } 
    }

    public class FiltersCall : Dictionary<string, FilterArgs>
    {
        
    }

    public class FilterArgs : Dictionary<string, string>
    {

    }
}