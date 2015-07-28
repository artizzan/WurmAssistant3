using System.IO;

namespace AldurSoft.WurmApi.Tests.Builders.WurmClient
{
    class WurmLogs
    {
        readonly DirectoryInfo logsDir;

        public WurmLogs(DirectoryInfo logsDir)
        {
            this.logsDir = logsDir;
        }
    }
}