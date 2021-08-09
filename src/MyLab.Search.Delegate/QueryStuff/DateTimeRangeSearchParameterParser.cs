using System.Linq;

namespace MyLab.Search.Delegate.QueryStuff
{
    class DateTimeRangeSearchParameterParser : ISearchParameterParser
    {
        public bool CanParse(string word)
        {
            var parts = word.Split('-');

            if (parts.Length != 2) return false;

            return parts.All(SupportedDateTimeFormat.CanParse);
        }

        public ISearchQueryParam Parse(string word, int rank)
        {
            var parts = word.Split('-');
            var from = SupportedDateTimeFormat.Parse(parts[0]);
            var to = SupportedDateTimeFormat.Parse(parts[1]);

            return new DateTimeRangeQueryParameter(from, to, rank);
        }
    }
}