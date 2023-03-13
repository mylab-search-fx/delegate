using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MyLab.Search.Searcher.Tools;
using Nest;

namespace MyLab.Search.Searcher.QueryTools
{
    class SearchQueryProcessor
    {
        private static readonly IQueryExpressionFactory[] Factories = 
        {
            new DateQueryExpressionFactory(), 
            new RangeDateQueryExpressionFactory(), 
            new GreaterThenDateQueryExpressionFactory(), 
            new LessThenDateQueryExpressionFactory(), 

            new NumericQueryExpressionFactory(), 
            new RangeNumericQueryExpressionFactory(), 
            new GreaterThenNumericQueryExpressionFactory(), 
            new LessThenNumericQueryExpressionFactory(),
            
            new WorldQueryExpressionFactory(), 
        };

        public IReadOnlyList<QueryItem> Items { get; }

        public SearchQueryProcessor(IEnumerable<QueryItem> items)
        {
            Items = new ReadOnlyCollection<QueryItem>(items.ToList());
        }

        public IEnumerable<QueryContainer> Process(TypeMapping mapping)
        {
            var queries = new List<QueryContainer>();

            if (mapping.Properties != null)
            {
                foreach (var queryItem in Items)
                {
                    var itemShouldQueries = new List<QueryContainer>();
                    
                    foreach (var property in mapping.Properties.Values)
                    {
                        if(!MappingPropertyTools.IsIndexed(property)) continue;

                        foreach (var itemExpression in queryItem.Expressions)
                        {
                            if (itemExpression.TryCreateQuery(property, out var q))
                            {
                                q.Name = $"itm-val:{property.Name.Name}:{queryItem.Name}";
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

                    queries.Add(boolQuery);
                }
            }

            return queries;
        }

        public static SearchQueryProcessor Parse(string queryString)
        {
            var items = new List<QueryItem>();

            if (queryString != null)
            {
                var literals = queryString
                    .Split(' ', '\t', StringSplitOptions.RemoveEmptyEntries)
                    .Where(l => l.Length > 2)
                    .ToArray();

                for (int i = 0; i < literals.Length; i++)
                {
                    var boost = 1 * (literals.Length - i);
                    var literal = literals[i];

                    var expressions = new List<IQueryExpression>();

                    foreach (var factory in Factories)
                    {
                        if (factory.TryCreate(literal, out var expr))
                        {
                            expressions.Add(expr);
                        }
                    }

                    if (expressions.Count != 0)
                    {
                        items.Add(new QueryItem(literal, boost, expressions));
                    }
                }
            }

            return new SearchQueryProcessor(items);
        }
    }

    class QueryItem
    {
        public IReadOnlyCollection<IQueryExpression> Expressions { get; }
        public int Boost { get; }
        public string Name { get; }

        public QueryItem(string name, int boost, IEnumerable<IQueryExpression> expressions)
        {
            Name = name;
            Boost = boost;
            Expressions = new ReadOnlyCollection<IQueryExpression>(expressions.ToList());
        }
    }
}
