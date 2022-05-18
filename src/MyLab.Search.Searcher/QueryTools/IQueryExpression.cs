using Nest;

namespace MyLab.Search.Searcher.QueryTools
{
    interface IQueryExpression
    {
        bool TryCreateQuery(IProperty property, out QueryBase query);
    }
}