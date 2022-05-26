namespace MyLab.Search.Searcher.Models
{
    public static class ClientSearchRequestV3Extensions
    {
        public static ClientSearchRequestV4 ToV4(this ClientSearchRequestV3 requestV3)
        {
            return new ClientSearchRequestV4
            {
                Filters = requestV3.Filters,
                Limit = requestV3.Limit,
                Offset = requestV3.Offset,
                Query = requestV3.Query,
                QuerySearchStrategy = requestV3.QuerySearchStrategy,
                Sort = requestV3.Sort
            };
        }
    }
}
