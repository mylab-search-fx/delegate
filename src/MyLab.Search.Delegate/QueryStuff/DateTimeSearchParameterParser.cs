namespace MyLab.Search.Delegate.QueryStuff
{
    class DateTimeSearchParameterParser : ISearchParameterParser
    {
        public bool CanParse(string word)
        {
            return SupportedDateTimeFormat.CanParse(word);
        }

        public ISearchQueryParam Parse(string word, int rank)
        {
            var dt = SupportedDateTimeFormat.Parse(word);

            return new DateTimeRangeQueryParameter(dt.Date, dt.Date.AddDays(1), rank);
        }
    }
}