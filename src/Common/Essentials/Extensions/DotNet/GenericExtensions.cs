using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AldursLab.Essentials.Extensions.DotNet
{
    public static class GenericExtensions
    {
        /// <summary>
        /// Returns true when 'obj' Equals any element from the 'allowed' params.
        /// If 'obj' Equals null and any 'allowed' element Equals null, also returns true.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="allowed"></param>
        /// <returns></returns>
        public static bool In<T>(this T obj, params T[] allowed)
        {
            if (allowed == null)
                allowed = new T[0];
            if (object.Equals(obj, null))
                return allowed.Any(arg => object.Equals(arg, null));
            else
                return allowed.Any(arg => obj.Equals(arg));
        }
    }
}
