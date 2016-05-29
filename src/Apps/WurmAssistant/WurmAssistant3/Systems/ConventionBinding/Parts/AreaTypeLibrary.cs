using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AldursLab.WurmAssistant3.Systems.ConventionBinding.Exceptions;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Systems.ConventionBinding.Parts
{
    public class AreaTypeLibrary
    {
        readonly IEnumerable<Assembly> assemblies;
        readonly List<AreaTypeReflectionInfo> areaTypes;

        public AreaTypeLibrary([NotNull] IEnumerable<Assembly> assemblies)
        {
            if (assemblies == null) throw new ArgumentNullException(nameof(assemblies));
            this.assemblies = assemblies;

            areaTypes = this.assemblies
                .SelectMany(assembly => assembly.GetTypes()
                                                .Where(type => type.Namespace != null)
                                                .Select(type => new AreaTypeReflectionInfo(type))
                                                .Where(info => info.AreaKnown))
                .ToList();
        }

        public IEnumerable<AreaTypeReflectionInfo> GetAllTypes() { return areaTypes; }
        public IEnumerable<AreaTypeReflectionInfo> GetAllConfigs() { return areaTypes.Where(info => info.IsAreaConfiguration); }

        public void Validate()
        {
            ThrowIfDuplicatedAreaConfigurations();
            ThrowIfAreaConfigurationsOutsideAreas();
        }

        void ThrowIfDuplicatedAreaConfigurations()
        {
            var groupedAreas = GetAllConfigs().GroupBy(config => config.AreaName);
            var duplicatedConfigs = groupedAreas.Where(grouping => grouping.Count() != 1).ToArray();
            if (duplicatedConfigs.Any())
            {
                throw new DuplicatedAreaConfigurationException(
                    duplicatedConfigs.ToDictionary(infos => infos.Key,
                        infos => infos.ToArray()));
            }
        }

        void ThrowIfAreaConfigurationsOutsideAreas()
        {
            var mislocatedAreaConfigurations = assemblies
                .SelectMany(assembly => assembly.GetTypes()
                                                .Where(type => type.Namespace != null)
                                                .Select(type => new AreaTypeReflectionInfo(type))
                                                .Where(info => !info.AreaKnown && info.IsAreaConfiguration))
                .ToList();

            if (mislocatedAreaConfigurations.Any())
            {
                throw new MislocatedAreaConfigurationException(mislocatedAreaConfigurations);
            }
        }
    }
}
