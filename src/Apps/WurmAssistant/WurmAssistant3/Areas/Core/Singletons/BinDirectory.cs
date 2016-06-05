using System;
using System.IO;
using System.Reflection;
using AldursLab.WurmAssistant3.Areas.Core.Contracts;
using Caliburn.Micro;

namespace AldursLab.WurmAssistant3.Areas.Core.Singletons
{
    public class BinDirectory : IBinDirectory, IHandle<string>
    {
        public BinDirectory()
        {
            FullPath = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().Location).LocalPath);
        }

        public string FullPath { get; private set; }
        public void Handle(string message)
        {
            
        }
    }
}