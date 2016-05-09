using System;
using System.IO;
using System.Reflection;
using AldursLab.WurmAssistant3.Areas.Core.Contracts;

namespace AldursLab.WurmAssistant3.Areas.Core.Components.Singletons
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