using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AldursLab.WurmAssistant.Shared
{
    public static class AppPaths
    {
        public static class WurmAssistant3
        {
            public static class DataDir
            {
                public static string FullPath { get; private set; }
                public static string LockFilePath { get; private set; }

                static DataDir()
                {
                    FullPath =
                        Path.Combine(
                            System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData),
                            "AldursLab",
                            "WurmAssistantData");
                    LockFilePath = Path.Combine(FullPath, "app.lock");
                }
            }
        }

        public static class WurmAssistantUnlimited
        {
            public static class DataDir
            {
                public static string FullPath { get; private set; }
                public static string LockFilePath { get; private set; }

                static DataDir()
                {
                    FullPath =
                        Path.Combine(
                            System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData),
                            "AldursLab",
                            "WurmAssistantUnlimitedData");
                    LockFilePath = Path.Combine(FullPath, "app.lock");
                }
            }
        }
    }
}
