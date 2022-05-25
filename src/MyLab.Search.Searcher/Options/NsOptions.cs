using System;

namespace MyLab.Search.Searcher.Options
{
    [Obsolete]
    public class NsOptions
    {
        public string Name { get; set; }
        public string Index { get; set; }
        public string DefaultFilter { get; set; }
        public string DefaultSort { get; set; }
        public int? DefaultLimit { get; set; }
        public QuerySearchStrategy QueryStrategy { get; set; }
    }
}