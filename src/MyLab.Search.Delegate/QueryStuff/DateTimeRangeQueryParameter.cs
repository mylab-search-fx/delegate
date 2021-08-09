using System;
using System.Globalization;

namespace MyLab.Search.Delegate.QueryStuff
{
    class DateTimeRangeQueryParameter : SearchQueryRangeParameter<DateTime>
    {
        static readonly DateTime Epoch = new DateTime(1970, 01, 01);

        public DateTimeRangeQueryParameter(DateTime? from, DateTime? to, int rank)
            : base(from, to, rank)
        {
        }

        public override string ToJson(string propName, string propType)
        {
            if (propType == "text")
                return null;

            return CreateRangeExpression(propName, true, false);
        }

        protected override string ValueToString(DateTime value)
        {
            return (value - Epoch).TotalMilliseconds.ToString(CultureInfo.InvariantCulture);
        }
    }
}