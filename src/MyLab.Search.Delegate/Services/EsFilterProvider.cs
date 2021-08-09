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

        public async Task<SearchFilter> ProvideAsync(string filterId)
        {
            var path = Path.Combine(_options.FilterPath, filterId + ".json");

            if (!File.Exists(path))
                throw new ResourceNotFoundException("Specified filter not found")
                    .AndFactIs("filterId", filterId);

            var str = await File.ReadAllTextAsync(path);

            return new SearchFilter
            {
                Content = str
            };
        }
    }
}