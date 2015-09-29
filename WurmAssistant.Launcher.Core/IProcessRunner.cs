using System.Diagnostics;
using System.IO;
using System.Linq;

namespace AldursLab.WurmAssistant.Launcher.Core
{
    public interface IProcessRunner
    {
        void Start(string filePath, string args);
    }

    public class ProcessRunner : IProcessRunner
    {
        public void Start(string filePath, string args)
        {
            string pathPart = string.Format("\"{0}\"", filePath);
            string argsPart = (args != null && args.Any()) ? (" " + args) : string.Empty;

            Process.Start(pathPart + argsPart);
        }
    }
}
