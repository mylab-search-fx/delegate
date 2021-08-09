using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MyLab.Search.Delegate.Models;
using MyLab.Search.EsAdapter;
using MyLab.Log;

namespace MyLab.Search.Delegate.Services
{
    class EsSortProvider : IEsSortProvider
    {
        private readonly DelegateOptions _options;

        public EsSortProvider(IOptions<DelegateOptions> options)
            :this(options.Value)
        {

        }

        public EsSortProvider(DelegateOptions options)
        {
            _options = options;
        }

        public async Task<SearchSort> ProvideAsync(string sortId)
        {
            var path = Path.Combine(_options.SortPath, sortId + ".json");

            if(!File.Exists(path))
                throw new ResourceNotFoundException("Specified sort not found")
                    .AndFactIs("sortId", sortId);

            var str = await File.ReadAllTextAsync(path);

            return new SearchSort
            {
                Content = str
            };
        }
    }
}