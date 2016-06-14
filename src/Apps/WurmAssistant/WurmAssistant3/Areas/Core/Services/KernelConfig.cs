using System;
using System.Linq;
using AldursLab.WurmAssistant3.Areas.Core.Contracts;
using AldursLab.WurmAssistant3.Utils.IoC;
using JetBrains.Annotations;
using Ninject;
using Ninject.Activation;
using Ninject.Activation.Strategies;
using Ninject.Planning.Bindings.Resolvers;

namespace AldursLab.WurmAssistant3.Areas.Core.Services
{
    class KernelConfig : IKernelConfig
    {
        readonly IKernel kernel;
        bool configured;
        readonly object locker = new object();

        public static IKernelConfig EnableFor(IKernel kernel)
        {
            return new KernelConfig(kernel);
        }

        KernelConfig([NotNull] IKernel kernel)
        {
            if (kernel == null) throw new ArgumentNullException(nameof(kernel));
            this.kernel = kernel;

            EnableCustomizations();
        }

        void EnableCustomizations()
        {
            lock (locker)
            {
                if (configured) return;

                var allComponents = kernel.Components.GetAll<IActivationStrategy>().ToList();
                if (allComponents.Count != 7)
                {
                    throw new Exception(
                        "Unexpected count of Ninject IActivationStrategy components in the Kernel, was the kernel already modified?");
                }
                kernel.Components.RemoveAll<IActivationStrategy>();
                kernel.Components.Add<IActivationStrategy, ActivationCacheStrategy>();
                kernel.Components.Add<IActivationStrategy, PropertyInjectionStrategy>();
                kernel.Components.Add<IActivationStrategy, MethodInjectionStrategy>();
                kernel.Components.Add<IActivationStrategy, PreInitializeActionsStrategy>();
                kernel.Components.Add<IActivationStrategy, InitializableStrategy>();
                kernel.Components.Add<IActivationStrategy, PostInitializeActionsStrategy>();
                kernel.Components.Add<IActivationStrategy, StartableStrategy>();
                kernel.Components.Add<IActivationStrategy, DisposableStrategy>();

                // Disable auto-binding of the types, that were not registered.
                // This helps in avoiding accidental resolve of concrete types instead of their interfaces.
                kernel.Components.Remove<IMissingBindingResolver, SelfBindingResolver>();

                configured = true;
            }
        }

        public void AddPreInitializeActivations(
            Action<IContext, InstanceReference> activationAction,
            Action<IContext, InstanceReference> deactivationAction)
        {
            lock (locker)
            {
                PreInitializeActionsStrategy str =
                    (PreInitializeActionsStrategy)
                        kernel.Components
                              .GetAll<IActivationStrategy>()
                              .Single(strategy => strategy is PreInitializeActionsStrategy);

                if (activationAction != null)
                { str.AddActivationAction(activationAction);}
                if (deactivationAction != null)
                { str.AddDeactivationAction(deactivationAction);}
            }
        }

        public void AddPostInitializeActivations(
            Action<IContext, InstanceReference> activationAction,
            Action<IContext, InstanceReference> deactivationAction)
        {
            lock (locker)
            {
                PostInitializeActionsStrategy str =
                    (PostInitializeActionsStrategy)
                        kernel.Components
                              .GetAll<IActivationStrategy>()
                              .Single(strategy => strategy is PostInitializeActionsStrategy);

                if (activationAction != null)
                { str.AddActivationAction(activationAction);}
                if (deactivationAction != null)
                { str.AddDeactivationAction(deactivationAction);}
            }
        }
    }
}
