using System;
using System.Linq;
using MyLab.Search.EsAdapter;
using MyLab.Log;

namespace MyLab.Search.Delegate
{
    public class DelegateOptions
    {
        public string SortPath { get; set; } = "/etc/mylab-search-delegate/sort/";
        public string FilterPath { get; set; } = "/etc/mylab-search-delegate/filter/";
        public Namespace[] Namespaces { get; set; }
        public Tokenizing Token { get; set; }
        public bool Debug { get; set; }
        public QuerySearchStrategy QueryStrategy { get; set; } = QuerySearchStrategy.Should;
        public string IndexNamePrefix { get; set; }
        public string IndexNamePostfix { get; set; }
        
        public class Namespace
        {
            public string Name { get; set; }
            public string Index { get; set; }
            public string DefaultFilter { get; set; }
            public string DefaultSort { get; set; }
            public int? DefaultLimit { get; set; }
            public QuerySearchStrategy QueryStrategy { get; set; }
        }

        public class Tokenizing
        {
            public int? ExpirySec { get; set; }
            public string SignKey { get; set; }
        }

        public Namespace GetNamespace(string ns)
        {
            var nsOptions = Namespaces?.FirstOrDefault(n => n.Name == ns);
            if (nsOptions == null)
                throw new InvalidOperationException("Namespace options not found")
                    .AndFactIs("ns", ns);

            return nsOptions;
        }

        public string GetIndexName(string ns)
        {
            var nsOptions = GetNamespace(ns);

            if(nsOptions.Index == null)
                throw new InvalidOperationException("Namespace index not defined")
                    .AndFactIs("ns", ns);

            return $"{IndexNamePrefix ?? string.Empty}{nsOptions.Index}{IndexNamePostfix ?? string.Empty}";
        }
    }
}
