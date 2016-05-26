using Ninject.Syntax;

namespace AldursLab.WurmAssistant3.Systems.ConventionBinding
{
    public static class NinjectExtensions
    {
        public static IBindingWithOrOnSyntax<T> WithOptionalAreaScopeArgument<T>(this IBindingWithSyntax<T> syntax)
        {
            return syntax.WithConstructorArgument(typeof(AreaScope),
                context =>
                {
                    var ns = context.Request.ParentContext?.Request.Service.Namespace;
                    return new AreaScope(ns ?? string.Empty);
                });
        }
    }
}