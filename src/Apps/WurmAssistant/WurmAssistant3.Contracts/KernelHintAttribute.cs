using System;

namespace AldursLab.WurmAssistant3
{
    /// <summary>
    /// Use this attribute to tell Kernel, that this class 
    /// should be bound using custom binding strategy.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class KernelHintAttribute : Attribute
    {
        public BindingHint BindingHint { get; }

        public KernelHintAttribute(BindingHint bindingHint)
        {
            BindingHint = bindingHint;
        }
    }

    public enum BindingHint
    {
        Singleton,
        DoNotBind
    }
}