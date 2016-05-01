using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

#if NETFX_CORE && !WinRT
#define WinRT
#endif

namespace AldursLab.Essentials.Extensions.DotNet
{
    public static class AttributeExtensions
    {
        /// <summary>
        /// Gets all the attributes of a particular type.
        /// </summary>
        /// <typeparam name="T">The type of attributes to get.</typeparam>
        /// <param name="member">The member to inspect for attributes.</param>
        /// <param name="inherit">Whether or not to search for inherited attributes.</param>
        /// <returns>The list of attributes found.</returns>
        public static IEnumerable<T> GetAttributes<T>(this MemberInfo member, bool inherit)
        {
#if WinRT
            return member.GetCustomAttributes(inherit).OfType<T>();
#else
            return Attribute.GetCustomAttributes(member, inherit).OfType<T>();
#endif
        }
    }
}
