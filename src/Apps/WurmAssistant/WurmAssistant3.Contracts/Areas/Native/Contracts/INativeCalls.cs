namespace AldursLab.WurmAssistant3.Areas.Native.Contracts
{
    public interface INativeCalls
    {
        void AttemptToBringMainWindowToFront(string processName, string windowTitleRegex);
    }
}