using System.IO;
using System.Text;
using System.Threading.Tasks;
using Elasticsearch.Net;
using Nest;

namespace MyLab.Search.Searcher.Tools
{
    static class EsSerializer
    {
        public static readonly IElasticsearchSerializer Instance = new ElasticClient().RequestResponseSerializer;

        public static Task<T> DeserializeAsync<T>(this IElasticsearchSerializer serializer, string sourceStr)
        {
            using var mem = new MemoryStream(Encoding.UTF8.GetBytes(sourceStr));
            return serializer.DeserializeAsync<T>(mem);
        }

        public static async Task<string> SerializeAsync<T>(this IElasticsearchSerializer serializer, T obj)
        {
            await using var mem = new MemoryStream();
            await serializer.SerializeAsync(obj, mem, SerializationFormatting.Indented);
            return Encoding.UTF8.GetString(mem.ToArray());
        }
    }
}
