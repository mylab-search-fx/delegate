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
        public bool HasQuery => Query is { HasQuery: true };
    }

    class SearchRequestPlanQuery
    {
        public QuerySearchStrategy Strategy { get; set; }
        public FilterRef[] Filters { get; set; }
        public SearchQueryApplier QueryApplier { get; set; }
        public bool HasQuery => Filters is { Length: > 0 } || QueryApplier != null;
    }
}
