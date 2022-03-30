using System.Linq;

namespace MyLab.Search.Delegate.Models
{
    static class TokenRequestV2Extensions
    {
        public static TokenRequestV3 ToV3(this TokenRequestV2 v2Req)
        {
            var req = new TokenRequestV3();

            if (v2Req.Namespaces != null)
            {
                req.Namespaces = v2Req.Namespaces.Select(ConvertNs).ToArray();
            }

            return req;
        }

        private static NamespaceSettingsV3 ConvertNs(NamespaceSettingsV2 arg)
        {
            return new NamespaceSettingsV3
            {
                Name = arg.Name,
                Filters = arg.Filters
            };
        }
    }
}
