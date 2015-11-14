namespace AldursLab.WurmAssistant3.Core.Areas.Native.Contracts
{
    public interface INativeCalls
    {
        void AttemptToBringMainWindowToFront(string processName, string windowTitleRegex);
    }
}