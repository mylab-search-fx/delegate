using System.Linq;

namespace MyLab.Search.Searcher.QueryTools
{
    class WorldQueryExpressionFactory : IQueryExpressionFactory
    {
        public bool TryCreate(string literal, out IQueryExpression queryExpression)
        {
            queryExpression = new WorldQueryExpression(literal);
            return true;
        }
    }
}