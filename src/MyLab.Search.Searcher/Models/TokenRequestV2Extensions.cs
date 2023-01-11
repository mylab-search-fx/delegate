using System.Linq;

namespace MyLab.Search.Searcher.Models
{
    static class TokenRequestV2Extensions
    {
        public static TokenRequestV4 ToV4(this TokenRequestV2 v2Req)
        {
            var req = new TokenRequestV4();

            if (v2Req.Namespaces != null)
            {
                req.Indexes = v2Req.Namespaces.Select(ConvertNs).ToArray();
            }

            return req;
        }

        private static IndexSettingsV4 ConvertNs(NamespaceSettingsV2 arg)
        {
            return new IndexSettingsV4
            {
                Id = arg.Name,
                Filters = arg.Filters
            };
        }
    }
}
