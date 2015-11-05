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
        public static string LockFileRelativePath = "app.lock";

        public static class WurmAssistant3
        {
            public static class DataDir
            {
                public static string FullPath { get; private set; }
                static DataDir()
                {
                    FullPath =
                        Path.Combine(
                            System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData),
                            "AldursLab",
                            "WurmAssistantData");
                }
            }
        }

        public static class WurmAssistantUnlimited
        {
            public static class DataDir
            {
                public static string FullPath { get; private set; }
                static DataDir()
                {
                    FullPath =
                        Path.Combine(
                            System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData),
                            "AldursLab",
                            "WurmAssistantUnlimitedData");
                }
            }
        }
    }
}
