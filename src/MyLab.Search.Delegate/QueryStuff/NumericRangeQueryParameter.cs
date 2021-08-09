namespace MyLab.Search.Delegate.QueryStuff
{
    class NumericRangeQueryParameter : SearchQueryRangeParameter<int>
    {
        public NumericRangeQueryParameter(int? from, int? to, int rank) 
            : base(from, to, rank)
        {
        }
    }
}