using System.Threading.Tasks;
using MyLab.ApiClient;

namespace MyLab.Search.Searcher.Client
{
    /// <summary>
    /// Describe searcher APIv4
    /// </summary>
    /// <remarks>
    /// Contract key = `searcher`
    /// </remarks>
    [Api("v4", Key = "searcher")]
    public interface ISearcherApiV4
    {
        /// <summary>
        /// Creates new search token
        /// </summary>
        [Post("token")]
        Task<string> CreateSearchTokenAsync([JsonContent] TokenRequestV4 tokenRequest);

        /// <summary>
        /// Performs searching
        /// </summary>
        /// <param name="indexId">index identifier</param>
        /// <param name="searchRequest">contains search parameters</param>
        /// <param name="searchToken">search token</param>
        [Post("indexes/{indexId}/searcher")]
        Task<FoundEntities<TRes>> SearchAsync<TRes>(
            [Path("indexId")] string indexId,
            [JsonContent] ClientSearchRequestV4 searchRequest,
            [Header("X-Search-Token")] string searchToken = null);
    }
}