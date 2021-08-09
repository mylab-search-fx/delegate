using System;

namespace MyLab.Search.Delegate.QueryStuff
{
    class SearchQueryRangeParameter<T> : ISearchQueryParam
        where T : struct
    {
        public T? From { get; }
        public T? To { get; }
        public int Rank { get; }

        public SearchQueryRangeParameter(T? from, T? to, int rank)
        {
            if(!from.HasValue && !to.HasValue)
                throw new InvalidOperationException("A least one range parameter should be specified");

            From = from;
            To = to;
            Rank = rank;
        }
    }
}