using System;
using System.IO;
using System.Reflection;
using AldursLab.WurmAssistant3.Core.Root.Contracts;

namespace AldursLab.WurmAssistant3.Core.Root.Components
{
    public class BinDirectory : IBinDirectory
    {
        public BinDirectory()
        {
            FullPath = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().Location).LocalPath);
        }

        public string FullPath { get; private set; }
    }
}