using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AldursLab.WurmAssistant3
{
    /// <summary>
    /// Use this attribute to tell kernel, that this interface 
    /// should be implemented with Ninject.Extensions.Factory auto-generated proxy.
    /// https://github.com/ninject/Ninject.Extensions.Factory/wiki
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface)]
    public class NinjectFactoryAttribute : Attribute
    {
    }
}
