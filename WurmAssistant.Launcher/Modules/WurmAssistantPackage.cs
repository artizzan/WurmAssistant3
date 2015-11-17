//using SevenZip;

namespace AldursLab.WurmAssistant.Launcher.Modules
{
    public interface IStagedPackage
    {
        void ExtractIntoDirectory(string targetDir);
    }
}