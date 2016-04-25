using System;

namespace AldursLab.WurmApi.Tests.Extensions
{
    public static class StringExtensions
    {
        public static string NormalizeLineEndings(this string s, LineEnding targetLineEnding = LineEnding.Lf)
        {
            if (targetLineEnding == LineEnding.Lf)
            {
                return s.Replace("\r\n", "\n");
            }
            else
            {
                throw new NotSupportedException();
            }
        }
    }

    public enum LineEnding
    {
        Lf
    }
}
