using MyLab.Search.Delegate.Tools;

namespace MyLab.Search.Delegate.QueryStuff
{
    abstract class SearchQueryParameter<T> : ISearchQueryParam
    {
        public T Value { get; }
        public int Rank { get;}
        protected SearchQueryParameter(T value, int rank)
        {
            Value = value;
            Rank = rank;
        }

        public string ToJson(string propName, string propType)
        {
            var boost = BoostCalculator.CalculateString(Rank);
            switch (propType)
            {
                case "text":
                    return "{\"match\":{\"" + propName + "\":{\"query\":\"" + Value + "\",\"boost\":" + boost + "}}}";
                default:
                    return "{\"term\":{\"" + propName + "\":{\"value\":\"" + Value + "\",\"boost\":" + boost + "}}}";
            }

        }
    }
}