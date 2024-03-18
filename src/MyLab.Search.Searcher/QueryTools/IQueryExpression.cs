using Nest;

namespace MyLab.Search.Searcher.QueryTools
{
    public interface IQueryExpression
    {
        bool TryCreateQuery(IProperty property, out QueryBase query);
    }
}