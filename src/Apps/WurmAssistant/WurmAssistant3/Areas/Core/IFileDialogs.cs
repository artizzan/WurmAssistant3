namespace AldursLab.WurmAssistant3.Areas.Core
{
    public interface IFileDialogs
    {
        string TryChooseAndReadTextFile();
        void SaveTextFile(string text);
    }
}