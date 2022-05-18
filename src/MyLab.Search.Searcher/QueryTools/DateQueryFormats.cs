using System.Collections.Generic;

namespace MyLab.Search.Searcher.QueryTools
{
    static class DateQueryFormats
    {
        public static readonly IEnumerable<string> Formats = new []
        {
            "d",
            "dd.MM.yyyy",
            "ddMMyy"
        };
    }
}