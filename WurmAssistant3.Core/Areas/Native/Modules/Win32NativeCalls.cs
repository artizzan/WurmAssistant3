using System;
using System.Diagnostics;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using AldursLab.WurmAssistant3.Core.Areas.Logging.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.Native.Constants;
using AldursLab.WurmAssistant3.Core.Areas.Native.Contracts;
using AldursLab.Essentials.Extensions.DotNet;

namespace AldursLab.WurmAssistant3.Core.Areas.Native.Modules
{
    class Win32NativeCalls : INativeCalls
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool ShowWindow(IntPtr hWnd, ShowWindowCommands nCmdShow);

        public void AttemptToBringMainWindowToFront(string processName, string windowTitleRegex)
        {
                var processes = Process.GetProcessesByName(processName);
                foreach (var process in processes)
                {
                    var handle = process.MainWindowHandle;
                    if (handle != IntPtr.Zero)
                    {
                        if (!string.IsNullOrEmpty(windowTitleRegex))
                        {
                            if (
                                !Regex.IsMatch(process.MainWindowTitle,
                                    windowTitleRegex,
                                    RegexOptions.CultureInvariant | RegexOptions.IgnoreCase))
                                continue;
                        }
                        ShowWindow(handle, ShowWindowCommands.Show);
                        ShowWindow(handle, ShowWindowCommands.Restore);
                        SetForegroundWindow(handle);
                    }
                }
        }
    }
}
