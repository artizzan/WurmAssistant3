using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject.Activation;

namespace AldursLab.WurmAssistant3.Areas.Core.Contracts
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
