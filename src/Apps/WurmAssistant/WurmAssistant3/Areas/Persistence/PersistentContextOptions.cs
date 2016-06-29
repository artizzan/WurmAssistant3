using AldursLab.Persistence;
using AldursLab.Persistence.Simple;

namespace AldursLab.WurmAssistant3.Areas.Persistence
{
    public sealed class PersistentContextOptions
    {
        /// <summary>
        /// If set, will be used instead of <see cref="DefaultJsonSerializer"/>.
        /// </summary>
        public ISerializer SerializerOverride { get; set; }

        /// <summary>
        /// If set, will be used instead of <see cref="FlatFilesDataStorage"/>.
        /// </summary>
        public IDataStorage DataStorageOverride { get; set; }

        /// <summary>
        /// If set, will disable auto saving feature.
        /// Autosaving attempts to save all pending changes every 15 seconds.
        /// </summary>
        public bool DisableAutoSaving { get; set; }
    }
}
