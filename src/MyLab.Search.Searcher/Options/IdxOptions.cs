using System;

namespace MyLab.Search.Searcher.Options
{
    public class IdxOptions
    {
        private string _id;
        private string _esIndex;

        public string Id
        {
            get => _id ?? Name;
            set => _id = value;
        }

        public string EsIndex
        {
            get => _esIndex ?? Index;
            set => _esIndex = value;
        }

        [Obsolete]
        public string Name { get; set; }
        [Obsolete]
        public string Index { get; set; }
        public string DefaultFilter { get; set; }
        public string DefaultSort { get; set; }
        public int? DefaultLimit { get; set; }
        public QuerySearchStrategy QueryStrategy { get; set; }

        public IdxOptions()
        {

        }

        public IdxOptions(NsOptions ns)
        {
            Id = ns.Name;
            EsIndex = ns.Index;
            DefaultFilter = ns.DefaultFilter;
            DefaultSort = ns.DefaultSort;
            DefaultLimit = ns.DefaultLimit;
            QueryStrategy = ns.QueryStrategy;
        }
    }
}