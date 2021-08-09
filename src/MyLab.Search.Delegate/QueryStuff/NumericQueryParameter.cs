namespace MyLab.Search.Delegate.QueryStuff
{
    class NumericQueryParameter : SearchQueryParameter<int>
    {
        public NumericQueryParameter(int value, int rank)
            : base(value, rank)
        {
        }
    }
}