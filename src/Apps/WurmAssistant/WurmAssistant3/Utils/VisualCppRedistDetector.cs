using Microsoft.Win32;

namespace AldursLab.WurmAssistant3.Utils
{
    public class VisualCppRedistDetector
    {
        public bool IsInstalled2010X86Sp1()
        {
            var key = Registry.CurrentUser.OpenSubKey(@"HKLM\SOFTWARE\Classes\Installer\Products\1D5E3C0FEDA1E123187686FED06E995A");
            return key != null;
        }
    }
}
