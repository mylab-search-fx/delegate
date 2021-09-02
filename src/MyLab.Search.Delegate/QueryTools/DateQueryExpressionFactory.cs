using System;
using System.Globalization;
using System.Linq;

namespace MyLab.Search.Delegate.QueryTools
{
    class DateQueryExpressionFactory : IQueryExpressionFactory
    {
        public bool TryCreate(string literal, out IQueryExpression queryExpression)
        {
            queryExpression = null;

            if (DateTime.TryParseExact(literal, DateQueryFormats.Formats.ToArray(), CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
            {
                queryExpression = new RangeDateQueryExpression(dt.Date, dt.Date.AddDays(1));
            }

            return queryExpression != null;
        }
    }
}