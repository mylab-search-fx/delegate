using System.Collections.Generic;
using Newtonsoft.Json;

namespace MyLab.Search.Delegate.Models
{
    public class TokenRequest
    {
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