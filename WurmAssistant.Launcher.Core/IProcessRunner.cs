using System.Diagnostics;

namespace AldursLab.WurmAssistant.Launcher.Core
{
    public interface IProcessRunner
    {
        void Start(string filePath);
    }

    public class ProcessRunner : IProcessRunner
    {
        public void Start(string filePath)
        {
            Process.Start(filePath);
        }
    }
}
