using System.Globalization;

namespace MyLab.Search.Delegate.Tools
{
    static class BoostCalculator
    {
        public static double Calculate(int rank)
        {
            return 1.0 + (0.1 * rank);
        }

        public static string CalculateString(int rank)
        {
            return Calculate(rank).ToString(CultureInfo.InvariantCulture);
        }
    }
}