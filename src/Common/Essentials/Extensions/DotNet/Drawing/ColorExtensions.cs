using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AldursLab.Essentials.Extensions.DotNet.Drawing
{
    public static class ColorExtensions
    {
        /// <summary>
        /// Gets either black or white color, depending which one contrasts better with input color.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static System.Drawing.Color GetContrastingBlackOrWhiteColor(this System.Drawing.Color input)
        {
            if (input.R + input.G + input.B < 3 * (256 / 2))
                return System.Drawing.Color.White;
            else
                return System.Drawing.Color.Black;
        }
    }
}
