using AldursLab.WurmAssistant.Shared;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.Core.Contracts
{
    /// <summary>
    /// Methods to find out version of the currently executing Build.
    /// </summary>
    public interface IWaVersion
    {
        [CanBeNull]
        Wa3VersionInfo VersionInfo { get; }

        string AsString();

        bool Known { get; }
    }
}