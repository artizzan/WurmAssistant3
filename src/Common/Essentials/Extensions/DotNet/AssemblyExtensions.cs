using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AldursLab.Essentials.Extensions.DotNet
{
    public static class AssemblyExtensions
    {
        public static string GetAssemblyLocationFullPath(this Assembly assembly)
        {
            return Path.GetDirectoryName(assembly.Location);
        }
    }
}
