using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AldursLab.Essentials.Extensions.DotNet.IO
{
    public static class PathExtensions
    {
        public static string CodeBaseLocalPath(this Assembly assembly)
        {
            return (Path.GetDirectoryName(new Uri(assembly.CodeBase).LocalPath));
        }
    }
}
