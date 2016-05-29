using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Ninject;

namespace AldursLab.WurmAssistant3.Systems.ConventionBinding.Parts
{
    class AreasBinder
    {
        readonly AreaTypeLibrary areaTypeLibrary;
        readonly IKernel kernel;
        readonly List<AreaBinder> areaBinders;

        public AreasBinder(
            [NotNull] AreaTypeLibrary areaTypeLibrary,
            [NotNull] IKernel kernel,
            [NotNull] IReadOnlyList<string> priorityOrderedAreas)
        {
            if (areaTypeLibrary == null) throw new ArgumentNullException(nameof(areaTypeLibrary));
            if (kernel == null) throw new ArgumentNullException(nameof(kernel));
            if (priorityOrderedAreas == null) throw new ArgumentNullException(nameof(priorityOrderedAreas));
            this.areaTypeLibrary = areaTypeLibrary;
            this.kernel = kernel;

            string[] allKnownAreaNames =
                areaTypeLibrary.GetAllTypes()
                               .Where(info => info.AreaKnown)
                               .Select(info => info.AreaName)
                               .Distinct()
                               .ToArray();

            var binderPool = allKnownAreaNames
                .Select(
                    areaName =>
                        new AreaBinder(
                            areaName,
                            areaTypeLibrary,
                            kernel))
                .ToList();

            List<AreaBinder> orderedBinders = new List<AreaBinder>();
            foreach (var areaName in priorityOrderedAreas)
            {
                var matchingManager = binderPool.SingleOrDefault(manager => manager.AreaName == areaName);
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
                areaBinder.BindByConvention();
                areaBinder.RunCustomConfigIfDefined();
            }
        }
    }
}