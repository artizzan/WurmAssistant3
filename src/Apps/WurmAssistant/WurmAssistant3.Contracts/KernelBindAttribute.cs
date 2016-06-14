using System;

namespace AldursLab.WurmAssistant3
{
    /// <summary>
    /// Use this attribute to tell Kernel, that this class 
    /// should be bound using custom binding strategy.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class KernelBindAttribute : Attribute
    {
        public BindingHint BindingHint { get; }

        public KernelBindAttribute(BindingHint bindingHint = BindingHint.Transient)
        {
            BindingHint = bindingHint;
        }
    }

    public enum BindingHint
    {
        Transient,
        Singleton,
        FactoryProxy
    }
}