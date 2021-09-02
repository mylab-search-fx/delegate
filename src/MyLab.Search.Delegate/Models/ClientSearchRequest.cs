using Microsoft.AspNetCore.Mvc;

namespace MyLab.Search.Delegate.Models
{
    public class ClientSearchRequest
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
    }
}
