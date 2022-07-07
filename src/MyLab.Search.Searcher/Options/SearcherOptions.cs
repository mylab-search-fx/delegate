using System;
using System.Linq;
using MyLab.Log;

namespace MyLab.Search.Searcher.Options
{
    public class SearcherOptions
    {
        public string SortPath { get; set; } = "/etc/mylab-search-searcher/sort/";
        public string FilterPath { get; set; } = "/etc/mylab-search-searcher/filter/";
        [Obsolete]
        public NsOptions[] Namespaces { get; set; }
        public IdxOptions[] Indexes { get; set; }
        public TokenizingOptions Token { get; set; }
        public bool Debug { get; set; }
        public QuerySearchStrategy QueryStrategy { get; set; } = QuerySearchStrategy.Should;
        [Obsolete]
        public string IndexNamePrefix { get; set; }
        public string EsIndexNamePrefix { get; set; }
        [Obsolete]
        public string IndexNamePostfix { get; set; }
        public string EsIndexNamePostfix { get; set; }
        
        public IdxOptions GetIndexOptions(string indexId)
        {
            var idxOptions = Indexes?.FirstOrDefault(n => n.Id == indexId);
            if (idxOptions == null)
            {

                var nsOptions = Namespaces?.FirstOrDefault(n => n.Name == indexId);

                if (nsOptions == null)
                    return null;

                idxOptions = new IdxOptions(nsOptions);

                throw new NamespaceConfigException(idxOptions)
                    .AndFactIs("index-id", indexId);
            }

            return idxOptions;
        }

        public string CreateEsIndexName(string idxId)
        {
            IdxOptions idxOptions;

            try
            {
                idxOptions = GetIndexOptions(idxId);
            }
            catch (NamespaceConfigException e)
            {
                idxOptions = e.IndexOptionsFromNamespaceOptions;
            }

            var totalEsIdxName = idxOptions?.EsIndex ?? idxId;

            return $"{EsIndexNamePrefix ?? IndexNamePrefix ?? string.Empty}{totalEsIdxName}{EsIndexNamePostfix ?? IndexNamePostfix ?? string.Empty}";
        }
    }
}
