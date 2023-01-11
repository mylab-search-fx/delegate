using System;
using System.Threading.Tasks;
using MyLab.ApiClient;

namespace MyLab.Search.SearcherClient
{
    /// <summary>
    /// Describe delegate APIv2
    /// </summary>
    /// <remarks>
    /// Contract key = `search-delegate`
    /// </remarks>
    [Api("v2", Key = "searcher")]
    [Obsolete]
    public interface ISearcherApiV2
    {
        /// <summary>
        /// Creates new search token
        /// </summary>
        [Post("token")]
        Task<string> CreateSearchTokenAsync([JsonContent] TokenRequestV2 tokenRequest);

        /// <summary>
        /// Performs searching
        /// </summary>
        /// <param name="ns">namespace</param>
        /// <param name="searchRequest">contains search parameters</param>
        /// <param name="searchToken">search token</param>
        [Post("search/{namespace}")]
        Task<FoundEntities<TRes>> SearchAsync<TRes>(
            [Path("namespace")] string ns,
            [JsonContent] ClientSearchRequestV2 searchRequest,
            [Header("X-Search-Token")] string searchToken = null);
    }
}