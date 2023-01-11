#if IS_CLIENT
namespace MyLab.Search.SearcherClient
#else
namespace MyLab.Search.Searcher
#endif
{
    public enum QuerySearchStrategy
    {
        Undefined,
        Should,
        Must
    }
}