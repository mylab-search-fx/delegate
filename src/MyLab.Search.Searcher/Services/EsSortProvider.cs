using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MyLab.Log;
using MyLab.Search.Searcher.Options;
using MyLab.Search.Searcher.Tools;
using Nest;

namespace MyLab.Search.Searcher.Services
{
    class EsSortProvider : IEsSortProvider
    {
        private readonly SearcherOptions _options;

        public EsSortProvider(IOptions<SearcherOptions> options)
            :this(options.Value)
        {

        }

        public EsSortProvider(SearcherOptions options)
        {
            _options = options;
        }

        public async Task<ISort> ProvideAsync(string sortId, string ns, IEnumerable<KeyValuePair<string, string>> args = null)
        {
            var sort = await ProvideCoreAsync(sortId, ns, args);

            if(sort == null)
                throw new ResourceNotFoundException("Specified sort not found")
                    .AndFactIs("sortId", sortId);

            return sort;
        }

        public Task<ISort> ProvideDefaultAsync(string ns)
        {
            return ProvideCoreAsync("default", ns);
        }

        async Task<ISort> ProvideCoreAsync(string sortId, string ns, IEnumerable<KeyValuePair<string, string>> args = null)
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
                    return null;
                }
            }

            var str = await File.ReadAllTextAsync(resultPath);

            if (args != null)
            {
                str = ApplyToRawSorting(args, str);
            }

            return await EsSerializer.Instance.DeserializeAsync<ISort>(str);
        }

        static string ApplyToRawSorting(IEnumerable<KeyValuePair<string, string>> args, string rawSorting)
        {
            var str = rawSorting;

            foreach (var sortingArg in args)
            {
                str = str.Replace("{" + sortingArg.Key + "}", sortingArg.Value);
            }

            return str;
        }
    }
}