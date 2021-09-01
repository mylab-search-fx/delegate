using System;
using System.Globalization;

namespace MyLab.Search.Delegate.QueryStuff
{
    static class SupportedDateTimeFormat
    {
        private static readonly string[] Formats =
        {
            "d",
            "dd.MM.yyyy",
            "ddMMyy"
        };

        public static bool CanParse(string str)
        {
            return DateTime.TryParseExact(str, Formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out _);
        }

        public static DateTime Parse(string str)
        {
            return DateTime.ParseExact(str, Formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
        }
    }
}