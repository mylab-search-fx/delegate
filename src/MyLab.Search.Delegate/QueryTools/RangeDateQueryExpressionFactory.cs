using System;
using System.Globalization;
using System.Linq;

namespace MyLab.Search.Delegate.QueryTools
{
    class RangeDateQueryExpressionFactory : IQueryExpressionFactory
    {
        public bool TryCreate(string literal, out IQueryExpression queryExpression)
        {
            queryExpression = null;

            var parts = literal.Split('-');

            if (parts.Length != 2) return false;

            if (DateTime.TryParseExact(parts[0], DateQueryFormats.Formats.ToArray(), CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt1)
                &&
                DateTime.TryParseExact(parts[1], DateQueryFormats.Formats.ToArray(), CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt2))
            {
                queryExpression = new RangeDateQueryExpression(dt1, dt2);
            }

            return queryExpression != null;
        }
    }
}