namespace MyLab.Search.Delegate.QueryStuff
{
    class SearchQueryParameter<T> : ISearchQueryParam
    {
        public T Value { get; }
        public int Rank { get;}

        public SearchQueryParameter(T value, int rank)
        {
            Value = value;
            Rank = rank;
        }
    }
}