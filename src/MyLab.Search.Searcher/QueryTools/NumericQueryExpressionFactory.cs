namespace MyLab.Search.Searcher.QueryTools
{
    class NumericQueryExpressionFactory : IQueryExpressionFactory
    {
        public bool TryCreate(string literal, out IQueryExpression queryExpression)
        {
            queryExpression = null;

            if (int.TryParse(literal, out var val))
            {
                queryExpression = new NumericQueryExpression(val);
            }

            return queryExpression != null;
        }
    }
}