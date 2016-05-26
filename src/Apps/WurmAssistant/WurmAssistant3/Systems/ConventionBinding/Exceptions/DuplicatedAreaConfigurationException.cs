using System;
using System.Collections.Generic;
using System.Linq;
using AldursLab.WurmAssistant3.Utils.IoC;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Systems.ConventionBinding.Exceptions
{
    [Serializable]
    public class DuplicatedAreaConfigurationException : ConventionBindingException
    {
        readonly IDictionary<string, AreaTypeReflectionInfo[]> duplicatedConfigs;

        public DuplicatedAreaConfigurationException(
            [NotNull] IDictionary<string, AreaTypeReflectionInfo[]> duplicatedConfigs)
        {
            if (duplicatedConfigs == null) throw new ArgumentNullException(nameof(duplicatedConfigs));
            this.duplicatedConfigs = duplicatedConfigs;
        }

        public override string ToString()
        {
            return
                $"There are multiple {nameof(IAreaConfiguration)} implementations within single AldursLab.WurmAssistant3.Areas subnamespace. "
                + $"Area can only have one {nameof(IAreaConfiguration)}. List of problems: " + Environment.NewLine
                + FormatProblems() + base.ToString();
        }

        string FormatProblems()
        {
            return string.Join("; ", duplicatedConfigs.Select(pair => $"Area: {pair.Key} Types: {FormatTypes(pair.Value)}" + Environment.NewLine));
        }

        string FormatTypes(AreaTypeReflectionInfo[] configs)
        {
            return string.Join(", ", configs.Select(info => info.Type.FullName));
        }
    }
}