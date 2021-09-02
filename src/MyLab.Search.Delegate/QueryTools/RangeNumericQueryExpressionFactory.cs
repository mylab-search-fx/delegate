namespace MyLab.Search.Delegate.QueryTools
{
    class RangeNumericQueryExpressionFactory : IQueryExpressionFactory
    {
        public bool TryCreate(string literal, out IQueryExpression queryExpression)
        {
            queryExpression = null;

            var parts = literal.Split('-');

            if (parts.Length != 2) return false;

            if (int.TryParse(parts[0], out var val1) &&
                int.TryParse(parts[1], out var val2))
            {
                queryExpression = new RangeNumericQueryExpression
                {
                    GreaterOrEqual = val1,
                    LessOrEqual = val2
                };
            }
            
            return queryExpression != null;
        }
    }
}