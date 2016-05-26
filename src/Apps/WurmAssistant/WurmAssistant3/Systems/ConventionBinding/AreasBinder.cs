using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Ninject;

namespace AldursLab.WurmAssistant3.Systems.ConventionBinding
{
    class AreasBinder
    {
        readonly IKernel kernel;
        readonly List<AreaBinder> areaBinders;

        public AreasBinder(
            [NotNull] IEnumerable<AreaTypeReflectionInfo> configurationTypes,
            [NotNull] IEnumerable<AreaReflectionInfo> detectedAreas,
            [NotNull] IKernel kernel,
            [NotNull] IReadOnlyList<string> priorityOrderedAreas)
        {
            if (configurationTypes == null) throw new ArgumentNullException(nameof(configurationTypes));
            if (detectedAreas == null) throw new ArgumentNullException(nameof(detectedAreas));
            if (kernel == null) throw new ArgumentNullException(nameof(kernel));
            if (priorityOrderedAreas == null) throw new ArgumentNullException(nameof(priorityOrderedAreas));
            this.kernel = kernel;

            var binderPool = detectedAreas
                .Select(
                    info =>
                        new AreaBinder(
                            info,
                            configurationTypes.SingleOrDefault(
                                reflectionInfo => reflectionInfo.AreaReflectionInfo.AreaName == info.AreaName)?.Type,
                            kernel))
                .ToList();

            List<AreaBinder> orderedBinders = new List<AreaBinder>();
            foreach (var areaName in priorityOrderedAreas)
            {
                var matchingManager = binderPool.SingleOrDefault(manager => manager.AreaReflectionInfo.AreaName == areaName);
                if (matchingManager == null)
                {
                    throw new InvalidOperationException($"Priority Area: {areaName} was not found among all reflected area types.");
                }
                orderedBinders.Add(matchingManager);
                binderPool.Remove(matchingManager);
            }

            foreach (var areaBinder in binderPool)
            {
                orderedBinders.Add(areaBinder);
            }

            areaBinders = orderedBinders;
        }

        public void SetupBindings()
        {
            foreach (var areaBinder in areaBinders)
            {
                var customConfig = areaBinder.TryActivateCustomConfig();
                areaBinder.BindByConvention();
                customConfig?.Configure(kernel);
            }
        }
    }
}