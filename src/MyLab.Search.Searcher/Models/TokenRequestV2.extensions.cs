using System.Linq;

namespace MyLab.Search.Searcher.Models
{
    public partial class TokenRequestV2
    {
        public TokenRequestV4 ToV4()
        {
            var req = new TokenRequestV4();

            if (Namespaces != null)
            {
                req.Indexes = Namespaces.Select(ConvertNs).ToArray();
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