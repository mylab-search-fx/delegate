using MyLab.Search.Searcher.QueryTools;
using Nest;

namespace MyLab.Search.Searcher.Models
{
    class SearchRequestPlan
    {
        public int? From { get; set; }
        public int? Size { get; set; }
        public SearchRequestPlanQuery Query { get; set; }
        public SortingRef Sorting { get; set; }
        public bool HasQueryOrSorting => Query is { HasQuery: true } || Sorting != null;
    }

    class SearchRequestPlanQuery
    {
        public QuerySearchStrategy Strategy { get; set; }
        public FilterRef[] Filters { get; set; }
        public SearchQueryProcessor QueryProcessor { get; set; }
        public bool HasQuery => Filters is { Length: > 0 } || QueryProcessor != null;
    }
}
