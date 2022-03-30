using System.Threading.Tasks;
using MyLab.ApiClient;

namespace MyLab.Search.Delegate.Client
{
    /// <summary>
    /// Describe delegate APIv2
    /// </summary>
    /// <remarks>
    /// Contract key = `search-delegate`
    /// </remarks>
    [Api("v3", Key = "search-delegate")]
    public interface ISearchDelegateApiV3
    {
        /// <summary>
        /// Creates new search token
        /// </summary>
        [Post("token")]
        Task<string> CreateSearchTokenAsync([JsonContent] TokenRequestV3 tokenRequest);

        /// <summary>
        /// Performs searching
        /// </summary>
        /// <param name="ns">namespace</param>
        /// <param name="searchRequest">contains search parameters</param>
        /// <param name="searchToken">search token</param>
        [Post("search/{namespace}")]
        Task<FoundEntities<TRes>> SearchAsync<TRes>(
            [Path("namespace")] string ns,
            [JsonContent] ClientSearchRequestV3 searchRequest,
            [Header("X-Search-Token")] string searchToken = null);
    }
}