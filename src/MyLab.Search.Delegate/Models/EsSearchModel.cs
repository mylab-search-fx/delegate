using MyLab.Search.Delegate.Tools;
using Newtonsoft.Json;

namespace MyLab.Search.Delegate.Models
{
    class EsSearchModel
    {
        [JsonProperty("min_score")]
        public float? MinScore{ get; set; }
        [JsonProperty("track_scores")]
        public bool TrackScores { get; set; } = true;
        [JsonProperty("from")]
        public int From { get; set; }
        [JsonProperty("size")]
        public int Size{ get; set; }

        [JsonProperty("query")]
        public EsSearchQueryModel Query { get; set; }
        [JsonProperty("sort")]
        [JsonConverter(typeof(StrToObjectArrayJsonConverter))]
        public string Sort { get; set; }
    }

    class EsSearchQueryModel
    {
        [JsonProperty("bool")]
        public EsSearchQueryBoolModel Bool { get; set; }
    }

    class EsSearchQueryBoolModel
    {
        [JsonProperty("minimum_should_match")]
        public int? MinShouldMatch { get; set; }

        [JsonProperty("should")]
        [JsonConverter(typeof(StrArrayToObjectArrayJsonConverter))]
        public string[] Should { get; set; }

        [JsonProperty("filter")]
        [JsonConverter(typeof(StrArrayToObjectArrayJsonConverter))]
        public string[] Filter { get; set; }
    }
}
