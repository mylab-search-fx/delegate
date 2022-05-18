namespace MyLab.Search.Searcher.QueryTools
{
    interface IQueryExpressionFactory
    {
        bool TryCreate(string literal, out IQueryExpression queryExpression);
    }
}