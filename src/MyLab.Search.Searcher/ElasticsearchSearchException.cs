using System;

namespace MyLab.Search.Searcher
{
    class ElasticsearchSearchException : Exception
    {
        public ElasticsearchSearchException(Exception inner) : base("An error occurred when ES search performed", inner)
        {
            
        }

        public ElasticsearchSearchException() : this(null)
        {

        }
    }
}
