using System.Linq;

namespace MyLab.Search.Delegate.QueryTools
{
    class WorldQueryExpressionFactory : IQueryExpressionFactory
    {
        public bool TryCreate(string literal, out IQueryExpression queryExpression)
        {
            queryExpression = null;

            if (!literal.Any(char.IsWhiteSpace))
            {
                queryExpression = new WorldQueryExpression(literal);
            }

            return queryExpression != null;
        }
    }
}