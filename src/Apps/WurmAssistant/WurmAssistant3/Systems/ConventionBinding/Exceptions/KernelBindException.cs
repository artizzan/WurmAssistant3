using System;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Systems.ConventionBinding.Exceptions
{
    public class KernelBindException : ConventionBindingException
    {
        readonly string message;
        readonly WaTypeInfo waTypeInfo;

        public KernelBindException([NotNull] WaTypeInfo waTypeInfo)
        {
            if (waTypeInfo == null) throw new ArgumentNullException(nameof(waTypeInfo));
            this.waTypeInfo = waTypeInfo;

            message =
                $"Type {waTypeInfo.Type.FullName} is defined as binding by {typeof(KernelBindAttribute)} but does not match any binding rules. "
                + "Binding rules are as follows: " + Environment.NewLine
                + $"{BindingHint.Singleton} and {BindingHint.Transient} must be a public non-abstract class."
                + Environment.NewLine
                + $"{BindingHint.FactoryProxy} should be a public interface.";
        }

        public KernelBindException([NotNull] string message, Exception innerException)
            : base(innerException)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            this.message = message;
        }

        public override string ToString()
        {
            return message + base.ToString();
        }
    }
}