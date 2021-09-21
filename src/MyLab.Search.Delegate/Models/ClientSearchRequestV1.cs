using System;
using Microsoft.AspNetCore.Mvc;

namespace MyLab.Search.Delegate.Models
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

        public ClientSearchRequestV2 ToV2()
        {
            var r = new ClientSearchRequestV2
            {
                Query = Query,
                QuerySearchStrategy = QuerySearchStrategy,
                Offset = Offset,
                Sort = Sort,
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