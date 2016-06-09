using System;
using System.Collections.Generic;
using System.Linq;
using AldursLab.WurmAssistant3.Areas.Features.Contracts;
using AldursLab.WurmAssistant3.Systems.ConventionBinding.Parts;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Systems.ConventionBinding.Exceptions
{
    [Serializable]
    public class MislocatedAreaConfigurationException : ConventionBindingException
    {
        readonly AreaTypeReflectionInfo[] mislocatedConfigs;

        public MislocatedAreaConfigurationException([NotNull] IEnumerable<AreaTypeReflectionInfo> mislocatedConfigs)
        {
            if (mislocatedConfigs == null) throw new ArgumentNullException(nameof(mislocatedConfigs));
            this.mislocatedConfigs = mislocatedConfigs.ToArray();
        }

        public override string ToString()
        {
            return
                $"At least one implementation of {nameof(IAreaConfiguration)} is not within AldursLab.WurmAssistant3.Areas subnamespace. "
                + "All configuration implementations must be within area they are intended to configure. List of mislocated types: "
                + string.Join(", ", mislocatedConfigs.Select(info => info.Type.FullName)) + " " + base.ToString();
        }
    }

    [Serializable]
    public class FeatureValidationException : ConventionBindingException
    {
        public FeatureValidationException(string message)
            : base(message)
        {
        }
    }
}