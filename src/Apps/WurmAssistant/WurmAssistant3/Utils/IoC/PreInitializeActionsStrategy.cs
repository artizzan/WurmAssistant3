using System;
using System.Collections.Generic;
using System.Linq;
using Ninject.Activation;
using Ninject.Activation.Strategies;

namespace AldursLab.WurmAssistant3.Utils.IoC
{
    public class PreInitializeActionsStrategy : ActivationStrategy, IActivationStrategy
    {
        List<Action<IContext, InstanceReference>> actions = new List<Action<IContext, InstanceReference>>();

        public void AddActivationAction(Action<IContext, InstanceReference> action)
        {
            var newList = actions.ToList();
            newList.Add(action);
            actions = newList;
        }

        public override void Activate(IContext context, InstanceReference reference)
        {
            foreach (var action in actions)
            {
                action(context, reference);
            }
        }

        public override void Deactivate(IContext context, InstanceReference reference)
        {
        }
    }
}