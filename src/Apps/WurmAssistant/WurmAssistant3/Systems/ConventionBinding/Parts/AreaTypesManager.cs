using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AldursLab.WurmAssistant3.Areas.Features.Contracts;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Systems.ConventionBinding.Parts
{
    public class AreaTypesManager
    {
        readonly AreaTypeReflectionInfo[] areaTypes;

        public AreaTypesManager([NotNull] string areaName, [NotNull] AreaTypeLibrary areaTypeLibrary)
        {
            if (areaName == null) throw new ArgumentNullException(nameof(areaName));
            if (areaTypeLibrary == null) throw new ArgumentNullException(nameof(areaTypeLibrary));
            AreaName = areaName;

            areaTypes = areaTypeLibrary.GetAllTypes().Where(info => info.MatchesAreaName(areaName)).ToArray();
        }

        public string AreaName { get; }

        public IEnumerable<Type> GetAllServices() { return FilterComponents("Services"); }
        public IEnumerable<Type> GetAllViewModels() { return FilterComponents("ViewModels"); }
        public IEnumerable<Type> GetAllAutoFactories()
        {
            return
                FilterComponents("Contracts").Where(type => type.GetCustomAttribute<NinjectFactoryAttribute>() != null);
        }

        IEnumerable<Type> FilterComponents(string componentType)
        {
            return areaTypes
                .Where(type => type.MatchesComponentType(componentType))
                .Select(info => info.Type);
        }

        public AreaTypeReflectionInfo TryGetAreaConfigurationType()
        {
            return areaTypes.SingleOrDefault(info => info.IsAreaConfiguration);
        }
    }
}