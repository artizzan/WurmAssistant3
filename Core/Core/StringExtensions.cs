using System;

namespace AldurSoft.Core
{
    /// <summary>
    /// Extensions for System.String
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Contains with StringComparison parameter.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="toCheck"></param>
        /// <param name="comp"></param>
        /// <returns></returns>
        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            if (toCheck == null) return false;
            return source.IndexOf(toCheck, comp) >= 0;
        }
    }
}
