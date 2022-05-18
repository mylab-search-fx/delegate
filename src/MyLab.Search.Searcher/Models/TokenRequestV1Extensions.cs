using System.Collections.Generic;
using System.Linq;

namespace MyLab.Search.Searcher.Models
{
    static class TokenRequestV1Extensions
    {
        public static TokenRequestV3 ToV3(this TokenRequestV1 v1Req)
        {
            var req = new TokenRequestV3();

            if (v1Req.Namespaces != null)
            {
                req.Namespaces = v1Req.Namespaces.Select(ConvertNs).ToArray();
            }

            return req;
        }

        private static NamespaceSettingsV3 ConvertNs(KeyValuePair<string, NamespaceSettingsV1> arg)
        {
            var settings = new NamespaceSettingsV3
            {
                Name = arg.Key,
                
            };

            if (arg.Value?.Filters != null)
            {
                settings.Filters = arg.Value.Filters.Select(f => new FilterRef
                {
                    Id = f.Key,
                    Args = f.Value
                }).ToArray();
            }

            return settings;
        }
    }
}
