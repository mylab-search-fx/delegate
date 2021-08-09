namespace MyLab.Search.Delegate.QueryStuff
{
    class NumericSearchParameterParser : ISearchParameterParser
    {
        public bool CanParse(string word)
        {
            return int.TryParse(word, out _);
        }

        public ISearchQueryParam Parse(string word, int rank)
        {
            return new NumericQueryParameter(int.Parse(word), rank);
        }
    }
}