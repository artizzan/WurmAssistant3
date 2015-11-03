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

            if (args != null)
            {
                Process.Start(pathPart, args);
            }
            else
            {
                Process.Start(pathPart);
            }
        }
    }
}
