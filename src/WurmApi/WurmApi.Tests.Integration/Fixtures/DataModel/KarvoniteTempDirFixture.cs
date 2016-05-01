using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WurmAssistant3.TestResources;

namespace Aldurcraft.WurmApi.Tests.Fixtures.DataModel
{
    class KarvoniteTempDirFixture : IDisposable
    {
        readonly EmptyDirectoryResourcePackage package = new EmptyDirectoryResourcePackage();

        public string DataStorePath { get { return Path.Combine(package.DirectoryFullPath, "DataStore.dat"); } }

        public void Dispose()
        {
            package.Dispose();
        }
    }
}
