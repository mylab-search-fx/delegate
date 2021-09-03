#if IS_CLIENT
namespace MyLab.Search.Delegate.Client
#else
namespace MyLab.Search.Delegate
#endif
{
    public enum QuerySearchStrategy
    {
        Undefined,
        Should,
        Must
    }
}