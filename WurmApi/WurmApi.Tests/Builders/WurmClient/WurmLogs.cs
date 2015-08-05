using System.IO;
using System.Text;

namespace AldurSoft.WurmApi.Tests.Builders.WurmClient
{
    class WurmLogs
    {
        readonly DirectoryInfo logsDir;

        public WurmLogs(DirectoryInfo logsDir)
        {
            this.logsDir = logsDir;
        }

        public WurmLogs CreateLogFile(string name, string content = null)
        {
            if (content == null) content = string.Empty;
            File.WriteAllText(Path.Combine(logsDir.FullName, name), content, Encoding.UTF8);
            return this;
        }
    }
}