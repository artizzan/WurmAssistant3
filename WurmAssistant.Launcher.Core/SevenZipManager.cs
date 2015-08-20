using System;
using System.IO;
using System.Reflection;

namespace AldursLab.WurmAssistant.Launcher.Core
{
    public static class SevenZipManager
    {
        static bool _set = false;
        public static void EnsurePathSet()
        {
            if (!_set)
            {
                var assemblyPath = Path.GetDirectoryName(Assembly.GetAssembly(typeof(SevenZipManager)).Location);
                if (assemblyPath == null)
                    throw new NullReferenceException("assemblyPath is null");

                var subDirPath = IntPtr.Size == 8 ? "x64" : "x86";
                var completePath = Path.Combine(assemblyPath, subDirPath, "7z.dll");

                SevenZip.SevenZipBase.SetLibraryPath(completePath);

                _set = true;
            }
        }
    }
}
