namespace MyLab.Search.Delegate.QueryStuff
{
    class NumericGreaterSearchParameterParser : ISearchParameterParser
    {
        public bool CanParse(string word)
        {
            return word.StartsWith(">") && int.TryParse(word.Substring(1), out _);
        }

        public ISearchQueryParam Parse(string word, int rank)
        {
            var val = int.Parse(word.Substring(1));
            return new NumericRangeQueryParameter(val, null, rank)
            {
                IncludeFrom = false
            };
        }
    }
}