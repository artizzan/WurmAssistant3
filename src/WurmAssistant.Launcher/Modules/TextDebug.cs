using System;
using System.IO;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant.Launcher.Modules
{
    public interface IDebug
    {
        void Write(string text);
        void Clear();
    }

    public class TextDebug : IDebug
    {
        readonly string filePath;

        public TextDebug([NotNull] string filePath)
        {
            if (filePath == null) throw new ArgumentNullException("filePath");
            this.filePath = filePath;
        }

        public void Write(string text)
        {
            File.AppendAllText(filePath, text);
        }

        public void Clear()
        {
            if (File.Exists(filePath)) File.Delete(filePath);
        }
    }
}
