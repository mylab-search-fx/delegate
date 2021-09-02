using System;
using Nest;

namespace MyLab.Search.Delegate.QueryTools
{
    class RangeDateQueryExpression : IQueryExpression
    {
        public DateTime? From { get; }
        public DateTime? To { get; }

        public RangeDateQueryExpression(DateTime? from, DateTime? to)
        {
            From = from;
            To = to;
        }

        public bool TryCreateQuery(IProperty property, out QueryBase query)
        {
            query = null;

            if (property.Type == "date")
            {
                query = new DateRangeQuery
                {
                    Field = property.Name.Name,
                    LessThan = To,
                    GreaterThanOrEqualTo = From
                };
            }

            return query != null;
        }
    }
}