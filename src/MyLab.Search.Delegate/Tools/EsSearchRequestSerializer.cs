using System.IO;
using System.Text;
using MyLab.Search.Delegate.Models;
using Newtonsoft.Json;

namespace MyLab.Search.Delegate.Tools
{
    class EsSearchRequestSerializer
    {
        private readonly JsonSerializer _serializer;
        
        public EsSearchRequestSerializer(bool indented = false)
        {
            _serializer = new JsonSerializer
            {
                Formatting = indented ? Formatting.Indented : Formatting.None,
                NullValueHandling = NullValueHandling.Ignore
            };
        }

        public string Serialize(EsSearchModel model)
        {
            var sb = new StringBuilder();
            using TextWriter writer = new StringWriter(sb);

            _serializer.Serialize(writer, model);

            return sb.ToString();
        }
    }
}
