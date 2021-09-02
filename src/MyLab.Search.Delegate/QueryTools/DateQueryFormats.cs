using System.Collections.Generic;

namespace MyLab.Search.Delegate.QueryTools
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