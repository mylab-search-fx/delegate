namespace MyLab.Search.Delegate.QueryStuff
{
    interface ISearchQueryParam
    {
        int Rank { get; }

        string ToJson(string propName, string propType);
    }
}