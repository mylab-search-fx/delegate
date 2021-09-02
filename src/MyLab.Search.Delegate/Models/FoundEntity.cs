using System.Collections.Generic;

#if IS_CLIENT
namespace MyLab.Search.Delegate.Client
#else
namespace MyLab.Search.Delegate.Models
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
