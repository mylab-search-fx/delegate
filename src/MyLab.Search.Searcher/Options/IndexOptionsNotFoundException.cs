using System;

namespace MyLab.Search.Searcher.Options
{
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