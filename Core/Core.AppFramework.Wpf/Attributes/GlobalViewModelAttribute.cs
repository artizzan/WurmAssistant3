using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.AppFramework.Wpf.Attributes
{
    /// <summary>
    /// Specifies, that decorated viewmodel should be global to the scope it is relevant for.
    /// For example singleton within an application or scoped to application module.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false)]
    public class GlobalViewModelAttribute : Attribute
    {
    }
}
