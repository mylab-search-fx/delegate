namespace MyLab.Search.Searcher.Models
{
    public partial class ClientSearchRequestV2
    {
        public ClientSearchRequestV4 ToV4()
        {
            return new ClientSearchRequestV4()
            {
                Query = Query,
                QuerySearchStrategy = QuerySearchStrategy,
                Offset = Offset,
                Sort = Sort != null
                    ? new SortingRef { Id = Sort }
                    : null,
                Limit = Limit,
                Filters = Filters
            };
        }
    }
}