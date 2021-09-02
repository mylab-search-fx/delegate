using MyLab.Search.Delegate.Models;

namespace MyLab.Search.Delegate.Tools
{
    static class FilterArgsExtensions
    {
        public static string ApplyToRowFilter(this FilterArgs args, string rawFilter)
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
