namespace MyLab.Search.Delegate.Models
{
    class SearchFilter
    {
        public static readonly SearchFilter MatchAll = new SearchFilter
        {
            Content = "{\"match_all\":{}}"
        };

        public string Content { get; set; }
    }
}