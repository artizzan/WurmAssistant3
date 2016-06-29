using System;

namespace AldursLab.WurmAssistant3.Areas.SoundManager
{
    /// <summary>
    /// Represents a single sound resource (eg. sound clip)
    /// </summary>
    public interface ISoundResource
    {
        Guid Id { get; }
        string Name { get; }
        float AdjustedVolume { get; }
        string FileFullName { get; }

        bool IsNull { get; }
    }
}