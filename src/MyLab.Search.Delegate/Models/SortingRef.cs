using System.Collections.Generic;
using Newtonsoft.Json;

#if IS_CLIENT
namespace MyLab.Search.Delegate.Client
#else
namespace MyLab.Search.Delegate.Models
#endif
{
    /// <summary>
    /// Reference to sorting
    /// </summary>
    public class SortingRef
    {
        /// <summary>
        /// Sorting identifier
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }
        /// <summary>
        /// Named sorting args
        /// </summary>
        [JsonProperty("args")]
        public Dictionary<string, string> Args { get; set; }
    }
}