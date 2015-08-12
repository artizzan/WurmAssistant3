using System.IO;
using System.Reflection;

namespace AldursLab.Deprec.Core
{
    public static class AssemblyExtensions
    {
        public static string GetCodeBasePath(this Assembly assembly)
        {
            var path = (new Uri(assembly.CodeBase)).LocalPath;
            path = Path.GetDirectoryName(path);
            return path;
        }
    }
}
