using System;
using System.Collections.Generic;
using System.Linq;

namespace MyLab.Search.Searcher.QueryTools
{
    public partial class SearchQueryApplier
    {
        public static SearchQueryApplier Parse(string queryString)
        {
            var items = new List<QueryItem>();
            QueryItem fullQueryItem = null;

            if (queryString != null)
            {
                var literals = queryString
                    .Split(' ', '\t', StringSplitOptions.RemoveEmptyEntries)
                    .Where(l => l.Length > 2)
                    .ToArray();


                if (literals.Length > 1)
                {
                    fullQueryItem = CreateExpressions(queryString, literals.Length + 1);
                }

                for (int i = 0; i < literals.Length; i++)
                {
                    var boost = literals.Length - i;
                    var literal = literals[i];

                    var item = CreateExpressions(literal, boost);
                    if (item != null)
                        items.Add(item);
                }
            }

            return new SearchQueryApplier(fullQueryItem, items);
        }

        private static QueryItem CreateExpressions(string literal, int boost)
        {
            var expressions = new List<IQueryExpression>();

            foreach (var factory in Factories)
            {
                if (factory.TryCreate(literal, out var expr))
                {
                    expressions.Add(expr);
                }
            }

            return expressions.Count != 0 ? new QueryItem(literal, boost, expressions) : null;
        }
    }
}