using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AldursLab.Testing.Extensions
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
