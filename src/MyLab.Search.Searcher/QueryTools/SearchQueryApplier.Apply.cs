using System;
using System.Collections.Generic;
using MyLab.Search.Searcher.Tools;
using Nest;

namespace MyLab.Search.Searcher.QueryTools
{
    public partial class SearchQueryApplier
    {
        public void Apply(TypeMapping mapping, QuerySearchStrategy strategy,
            BoolQuery parentQuery)
        {
            var queries = new List<QueryContainer>();

            FullItemsQueries(mapping, queries);

            var parentQueryChildren = new List<QueryContainer>();

            if (FullQueryItem != null)
            {
                var boolQuery = new BoolQuery();

                parentQueryChildren.Add(CreateItemQuery(mapping, FullQueryItem));
                parentQueryChildren.Add(boolQuery);

                switch (strategy)
                {
                    case QuerySearchStrategy.Should:
                    {
                        boolQuery.Should = queries;
                        boolQuery.MinimumShouldMatch = 1;
                    }
                        break;
                    case QuerySearchStrategy.Must:
                    {
                        boolQuery.Must = queries;
                    }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                parentQuery.Should = parentQueryChildren;
                parentQuery.MinimumShouldMatch = 1;
            }
            else
            {
                switch (strategy)
                {
                    case QuerySearchStrategy.Should:
                    {
                        parentQuery.Should = queries;
                        parentQuery.MinimumShouldMatch = 1;
                    }
                        break;
                    case QuerySearchStrategy.Must:
                    {
                        parentQuery.Must = queries;
                    }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void FullItemsQueries(TypeMapping mapping, List<QueryContainer> queries)
        {
            if (mapping.Properties != null)
            {
                foreach (var queryItem in Items)
                {
                    var boolQuery = CreateItemQuery(mapping, queryItem);

                    queries.Add(boolQuery);
                }
            }
        }

        private static BoolQuery CreateItemQuery(TypeMapping mapping, QueryItem queryItem)
        {
            var itemShouldQueries = new List<QueryContainer>();

            foreach (var property in mapping.Properties.Values)
            {
                if (!MappingPropertyTools.IsIndexed(property)) continue;

                foreach (var itemExpression in queryItem.Expressions)
                {
                    if (itemExpression.TryCreateQuery(property, out var q))
                    {
                        q.Name =
                            $"itm-val:{property.Name.Name}:{(queryItem.Name.Contains(' ') ? $"\"{queryItem.Name}\"" : queryItem.Name)}";
                        itemShouldQueries.Add(q);
                    }
                }
            }

            var boolQuery = new BoolQuery
            {
                Name = $"itm:{queryItem.Name}",
                Boost = queryItem.Boost,
                MinimumShouldMatch = 1,
                Should = itemShouldQueries
            };
            return boolQuery;
        }
    }
}