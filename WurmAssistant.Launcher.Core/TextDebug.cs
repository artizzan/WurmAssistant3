using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant.Launcher.Core
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
