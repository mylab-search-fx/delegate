using System;

namespace MyLab.Search.Delegate.QueryStuff
{
    class DateTimeQueryParameter : SearchQueryParameter<DateTime>
    {
        public DateTimeQueryParameter(DateTime value, int rank)
            : base(value, rank)
        {
        }
    }
}