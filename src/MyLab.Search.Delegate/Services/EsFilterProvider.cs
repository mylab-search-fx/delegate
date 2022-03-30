using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MyLab.Log;
using MyLab.Search.Delegate.Tools;
using Nest;

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

        public async Task<QueryContainer> ProvideAsync(string filterId, string ns, IEnumerable<KeyValuePair<string, string>> args = null)
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

            if (args != null)
            {
                str = ApplyToRawFilter(args, str);
            }

            return await EsSerializer.Instance.DeserializeAsync<QueryContainer>(str);
        }

        static string ApplyToRawFilter(IEnumerable<KeyValuePair<string,string>> args, string rawFilter)
        {
            var str = rawFilter;

            foreach (var filterArg in args)
            {
                str = str.Replace("{" + filterArg.Key + "}", filterArg.Value);
            }

            return str;
        }

    }
}