using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace AldursLab.Essentials.Extensions.DotNet
{
    public static class StringExtensions
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

        public static string Capitalize(this string source)
        {
            if (source == null) return null;
            for (int i = 0; i < source.Length; i++)
            {
                var x = source[i];
                if (!char.IsWhiteSpace(x))
                {
                    if (char.IsLetter(x))
                    {
                        var capitalized = char.ToUpper(x, CultureInfo.CurrentCulture);
                        return source.Substring(0, i) + capitalized + source.Substring(i + 1);
                    }
                }
            }
            return source;
        }

        /// <summary>
        /// Wraps the input text, using current platform new line character, 
        /// constraining each line to specified character count.
        /// </summary>
        /// <param name="source">string source</param>
        /// <param name="maxCharsPerLine">maximum number of characters per line</param>
        /// <returns></returns>
        public static string WrapLines(this string source, int maxCharsPerLine = 40)
        {
            return Regex.Replace(source,
                @"(.{" + maxCharsPerLine + @"}\s)",
                "$1" + Environment.NewLine,
                RegexOptions.Compiled);
        }
    }
}