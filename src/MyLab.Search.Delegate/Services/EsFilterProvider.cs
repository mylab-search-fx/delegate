using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MyLab.Log;
using MyLab.Search.Delegate.Models;

namespace MyLab.Search.Delegate.Services
{
    class EsFilterProvider : IEsFilterProvider
    {
        private readonly DelegateOptions _options;

        public EsFilterProvider(IOptions<DelegateOptions> options)
            : this(options.Value)
        {

        }

        public EsFilterProvider(DelegateOptions options)
        {
            _options = options;
        }

        public async Task<SearchFilter> ProvideAsync(string filterId, string ns)
        {
            var pathNs = Path.Combine(_options.FilterPath, ns, filterId + ".json");
            var pathBase = Path.Combine(_options.FilterPath, filterId + ".json");

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
                    throw new ResourceNotFoundException("Specified filter not found")
                        .AndFactIs("filterId", filterId);
                }
            }

            var str = await File.ReadAllTextAsync(resultPath);

            return new SearchFilter
            {
                Content = str
            };
        }
    }
}