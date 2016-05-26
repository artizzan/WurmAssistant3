using System;
using System.Collections.Generic;
using System.Linq;
using AldursLab.WurmAssistant3.Systems.ConventionBinding.Exceptions;
using AldursLab.WurmAssistant3.Utils.IoC;
using JetBrains.Annotations;
using Ninject;

namespace AldursLab.WurmAssistant3.Systems.ConventionBinding
{
    class ConventionBindingManager
    {
        // todo: this is not yet ready for plugins. 
        // To enable for plugins:
        // * Add plugin assembly search and merge types from all assemblies.
        // * Validate that all area types are within single Assembly (cross-assembly areas are not allowed)

        readonly IKernel kernel;
        readonly IReadOnlyList<string> priorityOrderedAreas;

        public ConventionBindingManager([NotNull] IKernel kernel, [NotNull] IReadOnlyList<string> priorityOrderedAreas)
        {
            if (kernel == null) throw new ArgumentNullException(nameof(kernel));
            if (priorityOrderedAreas == null) throw new ArgumentNullException(nameof(priorityOrderedAreas));
            this.kernel = kernel;
            this.priorityOrderedAreas = priorityOrderedAreas;
        }

        public void BindAreasByConvention()
        {
            var allConfigurations = ReflectAllAreaConfigs();

            var allTypesInAllAreas = ReflectAllAreaScopedTypes().ToArray();
            var allKnownAreas = allTypesInAllAreas.Select(info => info.AreaReflectionInfo).Distinct().ToArray();

            var manager = new AreasBinder(allConfigurations, allKnownAreas, kernel, priorityOrderedAreas);
            manager.SetupBindings();
        }

        IEnumerable<AreaTypeReflectionInfo> ReflectAllAreaConfigs()
        {
            var configurations = this.GetType()
                                     .Assembly.GetTypes()
                                     .Where(
                                         type =>
                                             type.Namespace != null
                                             && type.Namespace.StartsWith("AldursLab.WurmAssistant3.Areas")
                                             && type.IsSubclassOf(typeof(IAreaConfiguration)))
                                     .Select(type => new AreaTypeReflectionInfo(type))
                                     .ToArray();
            var mislocatedConfigs = configurations.Where(config => !config.AreaReflectionInfo.AreaKnown).ToArray();
            if (mislocatedConfigs.Any())
            {
                throw new MislocatedAreaConfigurationException(mislocatedConfigs);
            }

            var groupedAreas = configurations.GroupBy(config => config.AreaReflectionInfo.AreaName);
            var duplicatedConfigs = groupedAreas.Where(grouping => grouping.Count() != 1).ToArray();
            if (duplicatedConfigs.Any())
            {
                throw new DuplicatedAreaConfigurationException(
                    duplicatedConfigs.ToDictionary(infos => infos.Key,
                    infos => infos.ToArray()));
            }

            return configurations;
        }

        IEnumerable<AreaTypeReflectionInfo> ReflectAllAreaScopedTypes()
        {
            return
                this.GetType()
                    .Assembly.GetTypes()
                    .Select(type => new AreaTypeReflectionInfo(type))
                    .Where(info => info.AreaReflectionInfo.AreaKnown);
        }
    }
}
