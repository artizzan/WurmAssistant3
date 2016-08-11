using System;
using AldursLab.WurmAssistant3.Areas.Triggers.Data.Model;
using Caliburn.Micro;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.Triggers.ImportExport
{
    public class UserImportChoice
    {
        public UserImportChoice([NotNull] TriggerEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            Entity = entity;
        }

        public TriggerEntity Entity { get; }

        public bool ConflictResolution { get; set; }
    }

    public enum ConflictResolution
    {
        Skip,
        Replace,
        ImportAsNew
    }
}