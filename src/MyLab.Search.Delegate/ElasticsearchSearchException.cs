using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyLab.Search.Delegate
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
