using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyLab.Search.Delegate.Models
{
    static class TokenRequestV1Extensions
    {
        public static TokenRequestV2 ToV2(this TokenRequestV1 v1Req)
        {
            var req = new TokenRequestV2();

            if (v1Req.Namespaces != null)
            {
                req.Namespaces = v1Req.Namespaces.Select(ConvertNs).ToArray();
            }

            return req;
        }

        private static NamespaceSettingsV2 ConvertNs(KeyValuePair<string, NamespaceSettingsV1> arg)
        {
            var settings = new NamespaceSettingsV2
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
