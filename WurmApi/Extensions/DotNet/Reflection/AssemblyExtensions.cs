using System;
using System.IO;
using System.Reflection;

namespace AldursLab.WurmApi.Extensions.DotNet.Reflection
{
    static class AssemblyExtensions
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

        public static string GetAssemblyDllDirectoryAbsolutePath(this Assembly assembly)
        {
            return Path.GetDirectoryName(new Uri(assembly.CodeBase).LocalPath);
        }
    }
}
