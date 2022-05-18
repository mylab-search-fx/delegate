#if IS_CLIENT
namespace MyLab.Search.Searcher.Client
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