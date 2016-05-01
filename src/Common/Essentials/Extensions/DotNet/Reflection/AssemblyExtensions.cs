using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AldursLab.Essentials.Extensions.DotNet.Reflection
{
    public static class AssemblyExtensions
    {
        /// <summary>
        /// Get's the name of the assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>The assembly's name.</returns>
        public static string GetAssemblyName(this Assembly assembly)
        {
            return assembly.FullName.Remove(assembly.FullName.IndexOf(','));
        }
    }
}
