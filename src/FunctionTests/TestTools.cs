using System;
using System.Threading.Tasks;
using MyLab.Search.EsAdapter.Tools;

namespace FunctionTests
{
    static class TestTools
    {
        public static IAsyncDisposable IndexToolToAsyncDeleter(IEsIndexTool indexTool)
        {
            return new AsyncDisposableDeleter(indexTool);
        }

        class AsyncDisposableDeleter : IAsyncDisposable
        {
            private readonly IEsIndexTool _indexTool;

            public AsyncDisposableDeleter(IEsIndexTool indexTool)
            {
                _indexTool = indexTool;
            }

            public async ValueTask DisposeAsync()
            {
                await _indexTool.DeleteAsync();
            }
        }
    }
}
