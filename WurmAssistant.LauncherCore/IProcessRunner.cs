using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AldursLab.WurmAssistant.LauncherCore
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
