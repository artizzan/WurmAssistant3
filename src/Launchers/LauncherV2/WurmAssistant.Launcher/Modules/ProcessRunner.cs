using System.Diagnostics;
using AldursLab.WurmAssistant.Launcher.Contracts;

namespace AldursLab.WurmAssistant.Launcher.Modules
{
    public class ProcessRunner : IProcessRunner
    {
        public void Start(string filePath, string args)
        {
            string pathPart = string.Format("\"{0}\"", filePath);

            if (!string.IsNullOrWhiteSpace(args))
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