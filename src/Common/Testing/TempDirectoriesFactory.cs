using System.Collections.Concurrent;
using System.Text;
using System.Threading.Tasks;

namespace AldursLab.Testing
{
    public static class TempDirectoriesFactory
    {
        readonly static TempDirectoriesManager Manager = new TempDirectoriesManager();

        public static DirectoryHandle CreateEmpty()
        {
            return Manager.CreateEmpty();
        }

        public static DirectoryHandle CreateByCopy(string sourceDirectoryPath)
        {
            return Manager.CreateByCopy(sourceDirectoryPath);
        }

        public static DirectoryHandle CreateByUnzippingFile(string zipFilePath)
        {
            return Manager.CreateByUnzippingFile(zipFilePath);
        }
    }
}
