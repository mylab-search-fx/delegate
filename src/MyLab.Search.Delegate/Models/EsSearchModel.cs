﻿using MyLab.Search.Delegate.Tools;
using Newtonsoft.Json;

namespace MyLab.Search.Delegate.Models
{
    class EsSearchModel
    {
        [JsonProperty("from")]
        public int From { get; set; }
        [JsonProperty("size")]
        public int Size{ get; set; }

        [JsonProperty("query")]
        public EsSearchQueryModel Query { get; set; }
        [JsonProperty("sort")]
        [JsonConverter(typeof(StrArrayToObjectArrayJsonConverter))]
        public string[] Sort { get; set; }
    }

    class EsSearchQueryModel
    {
        [JsonProperty("bool")]
        public EsSearchQueryBoolModel Bool { get; set; }
    }

    class EsSearchQueryBoolModel
    {
        [JsonProperty("must")]
        [JsonConverter(typeof(StrArrayToObjectArrayJsonConverter))]
        public string[] Must { get; set; }
    }
}
