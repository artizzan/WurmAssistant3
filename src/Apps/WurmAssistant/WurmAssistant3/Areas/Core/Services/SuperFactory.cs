using System;
using AldursLab.WurmAssistant3.Areas.Core.Contracts;
using JetBrains.Annotations;
using Ninject;
using Ninject.Parameters;

namespace AldursLab.WurmAssistant3.Areas.Core.Services
{
    [KernelBind(BindingHint.Singleton)]
    public class SuperFactory : ISuperFactory
    {
        readonly IKernel kernel;

        public SuperFactory([NotNull] IKernel kernel)
        {
            if (kernel == null) throw new ArgumentNullException("kernel");
            this.kernel = kernel;
        }

        public T Get<T>()
        {
            return kernel.Get<T>();
        }

        public T Get<T>(params IParameter[] parameters)
        {
            return kernel.Get<T>(parameters);
        }
    }
}