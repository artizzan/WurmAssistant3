using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AldursLab.WurmAssistant3.Areas.Features.Contracts;
using AldursLab.WurmAssistant3.Systems.ConventionBinding.Exceptions;
using JetBrains.Annotations;
using Ninject;
using Ninject.Extensions.Factory;
using Ninject.Infrastructure.Language;
using Ninject.Syntax;

namespace AldursLab.WurmAssistant3.Systems.ConventionBinding.Parts
{
    class AreaBinder
    {
        AreaTypesManager AreaTypesManager { get; }

        [CanBeNull]
        readonly AreaTypeReflectionInfo areaConfigurationType;

        readonly IKernel kernel;

        public AreaBinder(
            [NotNull] string areaName,
            [NotNull] AreaTypeLibrary areaTypeLibrary,
            [NotNull] IKernel kernel)
        {
            if (areaName == null) throw new ArgumentNullException(nameof(areaName));
            if (areaTypeLibrary == null) throw new ArgumentNullException(nameof(areaTypeLibrary));
            if (kernel == null) throw new ArgumentNullException(nameof(kernel));
            AreaName = areaName;

            this.kernel = kernel;
            this.AreaTypesManager = new AreaTypesManager(areaName, areaTypeLibrary);
            this.areaConfigurationType = this.AreaTypesManager.TryGetAreaConfigurationType();
        }

        public string AreaName { get; }

        public void BindByConvention()
        {
            var services = AreaTypesManager.GetAllServices();
            foreach (var service in services)
            {
                BindContracts(service);
            }

            var viewModels = AreaTypesManager.GetAllViewModels();
            foreach (var viewModel in viewModels)
            {
                BindContracts(viewModel);
            }

            var autoFactories = AreaTypesManager.GetAllAutoFactories();
            foreach (var autoFactory in autoFactories)
            {
                if (!autoFactory.IsInterface)
                {
                    throw new InvalidOperationException(
                        $"Type {autoFactory.FullName} must be an interface in order to be properly proxied into an automatic Ninject factory.");
                }
                kernel.Bind(autoFactory).ToFactory(autoFactory);
            }
        }

        void BindContracts(Type service)
        {
            var contracts = GetAllImplementedServices(service).ToArray();
            var kernelHint = TryGetKernelHint(service);
            if (kernelHint != null)
            {
                if (kernelHint.BindingHint == BindingHint.Singleton)
                {
                    kernel.Bind(contracts.ToArray()).To(service).InSingletonScope().Named(service.FullName);
                }
                else if (kernelHint.BindingHint == BindingHint.DoNotBind)
                {
                    // do nothing
                }
                else
                {
                    kernel.Bind(contracts.ToArray()).To(service).Named(service.FullName);
                }
            }
            else
            {
                kernel.Bind(contracts.ToArray()).To(service).Named(service.FullName);
            }
        }
            
        IEnumerable<Type> GetAllImplementedServices(Type type)
        {
            List<Type> services = new List<Type> { type };
            var interfaces = type.GetInterfaces();
            if (interfaces.Any())
            {
                services.AddRange(interfaces);
            }
            var baseAbstractTypes = type.GetAllBaseTypes().Where(type1 => type1.IsAbstract).ToArray();
            if (baseAbstractTypes.Any())
            {
                services.AddRange(baseAbstractTypes);
            }

            return services;
        }

        KernelHintAttribute TryGetKernelHint(Type type)
        {
            return (KernelHintAttribute)type.GetCustomAttribute(typeof(KernelHintAttribute));
        }

        public void RunCustomConfigIfDefined()
        {
            if (areaConfigurationType != null)
            {
                var config = areaConfigurationType.ActivateAsAreaConfiguration();
                config.Configure(kernel);
            }
        }
    }
}