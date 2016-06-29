using System;
using Ninject.Activation;

namespace AldursLab.WurmAssistant3.Areas.Core
{
    public interface IKernelConfig
    {
        void AddPreInitializeActivations(
            Action<IContext, InstanceReference> activationAction,
            Action<IContext, InstanceReference> deactivationAction);

        void AddPostInitializeActivations(
            Action<IContext, InstanceReference> activationAction,
            Action<IContext, InstanceReference> deactivationAction);
    }
}
