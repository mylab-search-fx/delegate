using System.Linq;

namespace MyLab.Search.Delegate.QueryStuff
{
    class NumericRangeSearchParameterParser : ISearchParameterParser
    {
        public bool CanParse(string word)
        {
            var parts = word.Split('-');

            if (parts.Length != 2) return false;

            return parts.All(p => int.TryParse((string?) p, out _));
        }

        public ISearchQueryParam Parse(string word, int rank)
        {
            var parts = word.Split('-');
            int from = int.Parse(parts[0]);
            int to = int.Parse(parts[1]);

            return new NumericRangeQueryParameter(from, to, rank);
        }
    }
}