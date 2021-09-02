using System.Linq;

namespace MyLab.Search.Delegate.QueryTools
{
    static class NumericSupportedPropertyTypes
    {
        public static readonly string[] Types =
        {
            "long",
            "integer",
            "short",
            "byte",
            "double",
            "float",
            "half_float",
            "scaled_float",
            "unsigned_long"
        };

        public static bool IsSupported(string propertyType)
        {
            return Types.Contains(propertyType);
        }
    }
}