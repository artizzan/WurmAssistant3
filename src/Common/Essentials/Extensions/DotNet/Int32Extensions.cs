using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AldursLab.Essentials.Extensions.DotNet
{
    public static class Int32Extensions
    {
        public static int EnsureNonNegative(this int value)
        {
            return value < 0 ? 0 : value;
        }
    }
}
