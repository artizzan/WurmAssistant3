using System;
using System.Collections.Generic;
using System.Linq;
using Ninject.Activation;
using Ninject.Activation.Strategies;

namespace AldursLab.WurmAssistant3.Utils.IoC
{
    public class CustomActionsStrategyBase : ActivationStrategy, IActivationStrategy
    {
        List<Action<IContext, InstanceReference>> activationActions = new List<Action<IContext, InstanceReference>>();
        List<Action<IContext, InstanceReference>> deactivationActions = new List<Action<IContext, InstanceReference>>();

        public void AddActivationAction(Action<IContext, InstanceReference> action)
        {
            var newList = activationActions.ToList();
            newList.Add(action);
            activationActions = newList;
        }

        public void AddDeactivationAction(Action<IContext, InstanceReference> action)
        {
            var newList = deactivationActions.ToList();
            newList.Add(action);
            deactivationActions = newList;
        }

        public override void Activate(IContext context, InstanceReference reference)
        {
            foreach (var action in activationActions)
            {
                action(context, reference);
            }
        }

        public override void Deactivate(IContext context, InstanceReference reference)
        {
            foreach (var action in deactivationActions)
            {
                action(context, reference);
            }
        }
    }
}