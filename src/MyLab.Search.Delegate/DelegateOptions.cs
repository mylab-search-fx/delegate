using System;
using System.Linq;

namespace MyLab.Search.Delegate
{
    public class DelegateOptions
    {
        public string SortPath { get; set; } = "/etc/mylab-search-delegate/sort/";
        public string FilterPath { get; set; } = "/etc/mylab-search-delegate/filter/";
        public Namespace[] Namespaces { get; set; }

        public class Namespace
        {
            public string Name { get; set; }
            public string Index { get; set; }
            public string DefaultFilter { get; set; }
            public string DefaultSort { get; set; }
            public int? DefaultLimit { get; set; }
        }

        public Namespace GetNamespace(string ns)
        {
            var nsOptions = Namespaces?.FirstOrDefault(n => n.Name == ns);
            if (nsOptions == null)
                throw new InvalidOperationException("Namespace options not found");

            return nsOptions;
        }
    }
}
