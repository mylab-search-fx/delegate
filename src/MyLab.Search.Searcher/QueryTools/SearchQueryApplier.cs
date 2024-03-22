using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MyLab.Search.Searcher.Tools;
using Nest;

namespace MyLab.Search.Searcher.QueryTools
{
    public partial class SearchQueryApplier
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

        public QueryItem FullQueryItem { get; }

        public SearchQueryApplier(QueryItem fullQueryItem, IEnumerable<QueryItem> items)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));
            FullQueryItem = fullQueryItem;
            Items = new ReadOnlyCollection<QueryItem>(items.ToList());
        }
    }

    public class QueryItem
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
