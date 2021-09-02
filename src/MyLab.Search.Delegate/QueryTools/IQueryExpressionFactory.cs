namespace MyLab.Search.Delegate.QueryTools
{
    interface IQueryExpressionFactory
    {
        bool TryCreate(string literal, out IQueryExpression queryExpression);
    }
}