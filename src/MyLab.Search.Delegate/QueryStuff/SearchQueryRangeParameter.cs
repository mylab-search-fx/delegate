using System;
using System.Linq;
using Microsoft.AspNetCore.Http.Features;
using MyLab.Search.Delegate.Tools;

namespace MyLab.Search.Delegate.QueryStuff
{
    abstract class SearchQueryRangeParameter<T> : ISearchQueryParam
        where T : struct
    {
        public T? From { get; }
        public T? To { get; }
        public int Rank { get; }
        protected SearchQueryRangeParameter(T? from, T? to, int rank)
        {
            if(!from.HasValue && !to.HasValue)
                throw new InvalidOperationException("A least one range parameter should be specified");

            From = from;
            To = to;
            Rank = rank;
        }

        public abstract string ToJson(string propName, string propType);

        protected string CreateRangeExpression(string propName, bool includeLow = true, bool includeHigh = true)
        {
            var gtCondition = includeLow ? "gte" : "gt";
            var ltCondition = includeHigh ? "lte" : "lt";

            var gte = From.HasValue ? ("\"" + gtCondition + "\":" + ValueToString(From.Value)) : null;
            var lte = To.HasValue ? ("\"" + ltCondition + "\":" + ValueToString(To.Value)) : null;
            var boost = "\"boost\": " + BoostCalculator.CalculateString(Rank);

            var conditions = string.Join(',', new[] {gte, lte, boost}.Where(itm => itm != null));
            return "{\"range\":{\""+ propName + "\":{" + conditions + "}}}";
        }

        protected virtual string ValueToString(T value)
        {
            return value.ToString();
        }
    }
}