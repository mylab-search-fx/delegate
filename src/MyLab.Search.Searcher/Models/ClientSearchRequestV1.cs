using System;
using Microsoft.AspNetCore.Mvc;

namespace MyLab.Search.Searcher.Models
{
    [Obsolete]
    public class ClientSearchRequestV1 
    {
        [FromQuery(Name = "query")]
        public string Query { get; set; }
        [FromQuery(Name = "filter")]
        public string Filter { get; set; }
        [FromQuery(Name = "sort")]
        public string Sort { get; set; }
        [FromQuery(Name = "offset")]
        public int Offset { get; set; }
        [FromQuery(Name = "limit")]
        public int Limit { get; set; }
        [FromQuery(Name = "query-mode")]
        public QuerySearchStrategy QuerySearchStrategy { get; set; }

        public ClientSearchRequestV3 ToV3()
        {
            var r = new ClientSearchRequestV3
            {
                Query = Query,
                QuerySearchStrategy = QuerySearchStrategy,
                Offset = Offset,
                Sort = Sort != null
                    ? new SortingRef { Id = Sort }
                    : null,
                Limit = Limit
            };

            if (!string.IsNullOrEmpty(Filter))
            {
                r.Filters = new []
                {
                    new FilterRef
                    {
                        Id = Filter
                    }, 
                };
            }

            return r;
        }
    }
}