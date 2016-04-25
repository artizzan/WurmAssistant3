using System;
using System.IO;
using System.Reflection;

namespace AldursLab.WurmApi.Extensions.DotNet.IO
{
    static class PathExtensions
    {
        public static string CodeBaseLocalPath(this Assembly assembly)
        {
            return Path.GetDirectoryName(new Uri(assembly.CodeBase).LocalPath);
        }
    }
}
