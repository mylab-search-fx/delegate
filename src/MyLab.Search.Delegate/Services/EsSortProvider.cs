using System.Threading.Tasks;
using MyLab.Search.Delegate.Models;

namespace MyLab.Search.Delegate.Services
{
    class EsSortProvider : IEsSortProvider
    {
        public Task<SearchSort> ProvideAsync(string sortId)
        {
            throw new System.NotImplementedException();
        }
    }
}