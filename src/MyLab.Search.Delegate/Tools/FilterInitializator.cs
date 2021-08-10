using MyLab.Search.Delegate.Models;

namespace MyLab.Search.Delegate.Tools
{
    class FilterInitiator
    {
        private readonly FilterArgs _args;

        public FilterInitiator(FilterArgs args)
        {
            _args = args;
        }

        public void InitFilter(SearchFilter rawFilter)
        {
            string res = rawFilter.Content;

            foreach (var filterArg in _args)
            {
                res = res.Replace("{" + filterArg.Key + "}", filterArg.Value);
            }

            rawFilter.Content = res;
        }
    }
}
