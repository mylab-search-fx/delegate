using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MyLab.Search.Delegate.Models;
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

        public async Task<SearchSort> ProvideAsync(string sortId, string ns)
        {
            var pathNs = Path.Combine(_options.SortPath, ns, sortId + ".json");
            var pathBase = Path.Combine(_options.SortPath, sortId + ".json");

            string resultPath;

            if (File.Exists(pathNs))
            {
                resultPath = pathNs;
            }
            else
            {
                if (File.Exists(pathBase))
                {
                    resultPath = pathBase;
                }
                else
                {
                    throw new ResourceNotFoundException("Specified sort not found")
                        .AndFactIs("sortId", sortId);
                }
            }

            var str = await File.ReadAllTextAsync(resultPath);

            return new SearchSort
            {
                Content = str
            };
        }
    }
}