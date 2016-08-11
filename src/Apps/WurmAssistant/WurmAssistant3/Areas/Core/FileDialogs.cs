using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AldursLab.WurmAssistant3.Areas.Core
{
    [KernelBind(BindingHint.Singleton)]
    public class FileDialogs : IFileDialogs
    {
        public string TryChooseAndReadTextFile()
        {
            var dialog = new OpenFileDialog()
            {
                Multiselect = false,
                CheckFileExists = true,
                CheckPathExists = true,
                ValidateNames = true
            };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                var fileName = dialog.FileName;
                return File.ReadAllText(fileName);
            }

            return null;
        }

        public void SaveTextFile(string text)
        {
            var dialog = new SaveFileDialog()
            {
                CheckPathExists = true,
                ValidateNames = true,
                AddExtension = true,
                Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*"
            };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                var fileName = dialog.FileName;
                File.WriteAllText(fileName, text);
            }
        }
    }
}
