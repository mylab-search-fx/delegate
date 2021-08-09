using System.Threading.Tasks;
using MyLab.ApiClient;
using MyLab.Search.Delegate.Models;

namespace FunctionTests
{
    [Api]
    public interface ISearchService
    {
        [Get("search")]
        Task<TestEntity[]> SearchAsync(
            [Query] string query = null,
            [Query] string filter = null,
            [Query] string sort = null,
            [Query] int offset = 0,
            [Query] int limit = 0);
    }
}