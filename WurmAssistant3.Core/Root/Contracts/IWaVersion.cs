using AldursLab.WurmAssistant.Shared;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Core.Root.Contracts
{
    public interface IWaVersion
    {
        [CanBeNull]
        Wa3VersionInfo VersionInfo { get; }

        string AsString();

        bool Known { get; }
    }
}