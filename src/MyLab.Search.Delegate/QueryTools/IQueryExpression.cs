using Nest;

namespace MyLab.Search.Delegate.QueryTools
{
    interface IQueryExpression
    {
        bool TryCreateQuery(IProperty property, out QueryBase query);
    }
}