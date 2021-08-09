using System;

namespace MyLab.Search.Delegate.QueryStuff
{
    class DateTimeRangeQueryParameter : SearchQueryRangeParameter<DateTime>
    {
        public DateTimeRangeQueryParameter(DateTime? from, DateTime? to, int rank)
            : base(from, to, rank)
        {
        }
    }
}