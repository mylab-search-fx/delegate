using System.Text;
using Nest;

namespace MyLab.Search.Searcher.QueryTools
{
    class WorldQueryExpression : IQueryExpression
    {
        private static readonly string ReservedRegexChars = ".?+*|{}[]()\"\\#@&<>~";

        public string Literal { get; }
        public string LiteralRegex { get; }

        public WorldQueryExpression(string literal)
        {
            Literal = literal;
            LiteralRegex = ToRegex(literal);
        }

        public bool TryCreateQuery(IProperty property, out QueryBase query)
        {
            query = null;

            if (property.Type == "keyword")
            {
                query = new MatchQuery
                {
                    Field = property.Name.Name,
                    Query = Literal,
                    Analyzer = "whitespace"
                };
            }

            if (property.Type == "text")
            {
                query = new BoolQuery
                {
                    MinimumShouldMatch = 1,
                    Should = new QueryContainer[]
                    {
                        new MatchQuery
                        {
                            Field = property.Name.Name,
                            Query = Literal,
                            Analyzer = "whitespace"
                        },
                        new RegexpQuery
                        {
                            Field = property.Name.Name,
                            Value = LiteralRegex.ToLower(),
                        }
                    }
                };
            }

            return query != null;
        }

        private string ToRegex(string literal)
        {
            var b = new StringBuilder();
            var chars = literal.ToCharArray();

            foreach (var ch in chars)
            {
                b.Append(
                    ReservedRegexChars.Contains(ch)
                        ? $"\\{ch}"
                        :ch.ToString()
                );
            }

            b.Append(".*");

            return b.ToString();
        }
    }
}