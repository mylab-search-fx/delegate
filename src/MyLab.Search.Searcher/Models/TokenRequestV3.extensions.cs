using System.Linq;
using Newtonsoft.Json;

namespace MyLab.Search.Searcher.Models

{
    public static class TokenRequestV3Extensions
    {
        public static TokenRequestV4 ToV4(this TokenRequestV3 requestV3)
        {
            return new TokenRequestV4
            {
                Indexes = requestV3?.Namespaces.Select(n => new IndexSettingsV4
                {
                    Id = n.Name,
                    Filters = n.Filters
                }).ToArray()
            };
        }
    }
}