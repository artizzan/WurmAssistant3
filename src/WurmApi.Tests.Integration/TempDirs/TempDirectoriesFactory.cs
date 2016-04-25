namespace AldursLab.WurmApi.Tests.TempDirs
{
    public static class TempDirectoriesFactory
    {
        static readonly TempDirectoriesManager Manager = new TempDirectoriesManager();

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
