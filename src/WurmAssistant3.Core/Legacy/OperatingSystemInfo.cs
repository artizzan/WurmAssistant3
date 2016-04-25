using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AldursLab.WurmAssistant3.Core.Legacy
{
    /// <summary>
    /// Utility intended to detect operating system type, currently discerns: WinXP and Other
    /// </summary>
    /// <remarks>
    /// Many WinForms controls look or work differently in WinXP compared to Win7 and Win8, 
    /// this helps customizing GUI code.
    /// </remarks>
    public static class OperatingSystemInfo
    {
        const string THIS = "OperatingSystemInfo";

        public enum OStype { Unknown, WinXP, Other }

        public static OStype RunningOS { get; private set; }
        public static string RunningOS_Raw { get; private set; }

        static OperatingSystemInfo()
        {
            try
            {
                System.OperatingSystem os = Environment.OSVersion;
                var platform = Environment.OSVersion.Platform;
                var version = Environment.OSVersion.Version;

                RunningOS_Raw = platform.ToString() + " " + version.ToString();

                if (platform == PlatformID.Win32NT && version.Major == 5 && version.Minor == 1)
                    RunningOS = OStype.WinXP;
                else
                    RunningOS = OStype.Other;
            }
            catch (Exception)
            {
                RunningOS = OStype.Unknown;
            }
        }
    }
}
