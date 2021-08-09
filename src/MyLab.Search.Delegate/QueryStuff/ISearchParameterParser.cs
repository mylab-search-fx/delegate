namespace MyLab.Search.Delegate.QueryStuff
{
    interface ISearchParameterParser
    {
        bool CanParse(string word);
        ISearchQueryParam Parse(string word, int rank);
    }
}