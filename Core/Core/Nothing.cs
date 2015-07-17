using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AldurSoft.Core
{
    /// <summary>
    /// Represents nothing at all.
    /// </summary>
    public class Nothing
    {
        private Nothing() { }

        // ReSharper disable once InconsistentNaming
        private static readonly Nothing instance = new Nothing();
        public static Nothing Instance { get { return instance; } }
    }
}
