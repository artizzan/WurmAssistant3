using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Systems.ConventionBinding.Exceptions
{
    [Serializable]
    public class AreaTypesSpreadAcrossAssembliesException : ConventionBindingException
    {
        readonly IDictionary<string, IEnumerable<Assembly>> assemblySpreadInfo;

        public AreaTypesSpreadAcrossAssembliesException(
            [NotNull] IDictionary<string, IEnumerable<Assembly>> assemblySpreadInfo)
        {
            if (assemblySpreadInfo == null) throw new ArgumentNullException(nameof(assemblySpreadInfo));
            this.assemblySpreadInfo = assemblySpreadInfo;
        }

        public override string ToString()
        {
            return
                $"Detected that at least one area has types declared within multiple assemblies. This is not allowed. "
                + $"List of problems: " + Environment.NewLine
                + FormatProblems() + base.ToString();
        }

        string FormatProblems()
        {
            return string.Join("; ", assemblySpreadInfo.Select(pair => $"Area: {pair.Key} Assemblies: {FormatTypes(pair.Value)}" + Environment.NewLine));
        }

        string FormatTypes(IEnumerable<Assembly> assemblies)
        {
            return string.Join(", ", assemblies.Select(assembly => assembly.FullName));
        }
    }
}