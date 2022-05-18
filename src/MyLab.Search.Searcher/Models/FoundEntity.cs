#if IS_CLIENT
namespace MyLab.Search.Searcher.Client
#else
namespace MyLab.Search.Searcher.Models
#endif
{
    public class FoundEntity<T>
    {
        public T Content { get; set; }
        public double? Score { get; set; }
        public object Explanation { get; set; }
    }

    public class FoundEntities<T>
    {
        public FoundEntity<T>[] Entities { get; set; }

        public long Total { get; set; }

        public object EsRequest { get; set; }
    }
}
