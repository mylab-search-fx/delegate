using Nest;

namespace MyLab.Search.Delegate.QueryStuff
{
    class TextSearchParameterParser : ISearchParameterParser
    {
        public bool CanParse(string word)
        {
            return true;
        }

        public ISearchQueryParam Parse(string word, int rank)
        {
            return new TextQueryParameter(word, rank);
        }
    }
}