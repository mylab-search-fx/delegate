namespace MyLab.Search.Delegate.QueryStuff
{
    class NumericRangeQueryParameter : SearchQueryRangeParameter<int>
    {
        public bool IncludeFrom { get; set; }
        public bool IncludeTo { get; set; }

        public NumericRangeQueryParameter(int? from, int? to, int rank) 
            : base(from, to, rank)
        {
        }

        public override string ToJson(string propName, string propType)
        {
            if (propType == "text")
                return null;

            return CreateRangeExpression(propName, IncludeFrom, IncludeTo);
        }
    }
}