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
                {
                    throw new IndexOptionsNotFoundException(indexId)
                        .AndFactIs("index-id", indexId);
                }

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

            return $"{EsIndexNamePrefix ?? IndexNamePrefix ?? string.Empty}{idxOptions.EsIndex}{EsIndexNamePostfix ?? IndexNamePostfix ?? string.Empty}";
        }
    }

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

    public class TokenizingOptions
    {
        public int? ExpirySec { get; set; }
        public string SignKey { get; set; }
    }

    class NamespaceConfigException : Exception
    {
        public IdxOptions IndexOptionsFromNamespaceOptions { get; }

        public NamespaceConfigException(IdxOptions indexOptionsFromNamespaceOptions)
            : base("An old config with 'namespaces' instead of 'indexes' detected")
        {
            IndexOptionsFromNamespaceOptions = indexOptionsFromNamespaceOptions;
        }
    }

    class IndexOptionsNotFoundException : Exception
    {
        public string IndexName { get; }

        public IndexOptionsNotFoundException(string indexName)
            : base("Index options not found")
        {
            IndexName = indexName;
        }
    }
}
