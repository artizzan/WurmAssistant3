using System;
using System.IO;
using System.Windows;
using AldursLab.WurmAssistant3.Areas.Config.Views;

namespace AldursLab.WurmAssistant3.Utils
{
    class IrrklangDependencyValidator
    {
        public bool HandleWhenMissingIrrklangDependency(Exception exception)
        {
            // Trying to handle missing irrKlang dependencies.
            // Checking for specific part of the message, because the rest of it may be localized.
            if (exception.GetType() == typeof(FileNotFoundException)
                && exception.Message.Contains("'irrKlang.NET4.dll'"))
            {
                var visualCppDetector = new VisualCppRedistDetector();
                // trying to detect if Visual C++ Redistributable x86 SP1 is installed
                if (!visualCppDetector.IsInstalled2010X86Sp1())
                {
                    var form = new VisualCppMissingHelperView(exception);
                    form.ShowDialog();
                    Application.Current.Shutdown();
                    return true;
                }
            }

            return false;
        }
    }
}