using System;
using System.Globalization;

namespace AldursLab.WurmApi.Extensions.DotNet
{
    static class StringExtensions
    {
        public static string FormatInvariant(this string format, params object[] args)
        {
            return string.Format(CultureInfo.InvariantCulture, format, args);
        }

        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        public static bool IsNotNullAndNotEmpty(this string str)
        {
            return !IsNullOrEmpty(str);
        }

        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            if (toCheck == null)
                return false;
            return source.IndexOf(toCheck, comp) >= 0;
        }
    }
}