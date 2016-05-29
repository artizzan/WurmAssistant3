using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AldursLab.WurmAssistant3.Systems.ConventionBinding.Exceptions;
using AldursLab.WurmAssistant3.Systems.ConventionBinding.Parts;
using AldursLab.WurmAssistant3.Utils.IoC;
using JetBrains.Annotations;
using Ninject;

namespace AldursLab.WurmAssistant3.Systems.ConventionBinding
{
    /// <summary>
    /// </summary>
    /// <remarks>
    /// All types are expected to be strictly within AldursLab.WurmAssistant3.Areas.[AreaName] namespaces.
    /// </remarks>
    public class ConventionBindingManager
    {
        readonly IKernel kernel;
        readonly IReadOnlyList<string> priorityOrderedAreas;

        readonly AreaTypeLibrary areaTypeLibrary;

        public ConventionBindingManager([NotNull] IKernel kernel, [NotNull] IReadOnlyList<string> priorityOrderedAreas,
            [NotNull] IReadOnlyList<Assembly> assemblies)
        {
            if (kernel == null) throw new ArgumentNullException(nameof(kernel));
            if (priorityOrderedAreas == null) throw new ArgumentNullException(nameof(priorityOrderedAreas));
            if (assemblies == null) throw new ArgumentNullException(nameof(assemblies));
            this.kernel = kernel;
            this.priorityOrderedAreas = priorityOrderedAreas;

            areaTypeLibrary = new AreaTypeLibrary(assemblies);
        }

        public void BindAreasByConvention()
        {
            areaTypeLibrary.Validate();

            var manager = new AreasBinder(areaTypeLibrary, kernel, priorityOrderedAreas);
            manager.SetupBindings();
        }
    }
}
