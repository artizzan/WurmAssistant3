namespace AldursLab.WurmAssistant3.Areas.Native
{
    public interface INativeCalls
    {
        void AttemptToBringMainWindowToFront(string processName, string windowTitleRegex);
    }
}