using System;
using System.Linq;
using System.Reflection;
using Caliburn.Micro;

namespace AldursLab.WurmAssistant3.Infrastructure
{
    static class CaliburnConventionsHelper
    {
        public static Assembly[] MapConventionsForAssemblies(Type[] viewAssemblyRootAnchors, Type[] viewModelAssemblyRootAnchors)
        {
            if (viewAssemblyRootAnchors == null) throw new ArgumentNullException("viewAssemblyRootAnchors");
            if (viewAssemblyRootAnchors.Any(type => type == null))
                throw new ArgumentException("viewAssemblyRootAnchors contains null elements");
            if (viewModelAssemblyRootAnchors == null) throw new ArgumentNullException("viewModelAssemblyRootAnchors");
            if (viewModelAssemblyRootAnchors.Any(type => type == null))
                throw new ArgumentException("viewModelAssemblyRootAnchors contains null elements");

            foreach (var viewAssemblyRootAnchor in viewAssemblyRootAnchors)
            {
                foreach (var viewModelAssemblyRootAnchor in viewModelAssemblyRootAnchors)
                {
                    //{
                    //    var vmNs = viewModelAssemblyRootAnchor.Namespace + ".ViewModels";
                    //    var vNs = viewAssemblyRootAnchor.Namespace + ".Views";
                    //    ViewLocator.AddNamespaceMapping(vmNs, vNs);
                    //    ViewModelLocator.AddNamespaceMapping(vNs, vmNs);
                    //}

                    {
                        var vmNs = viewModelAssemblyRootAnchor.Namespace + ".ViewModels";
                        var vNs = viewAssemblyRootAnchor.Namespace + ".Views";
                        ViewLocator.AddSubNamespaceMapping(vmNs, vNs);
                        ViewModelLocator.AddSubNamespaceMapping(vNs, vmNs);
                    }
                }
            }

            return
                viewAssemblyRootAnchors.Concat(viewModelAssemblyRootAnchors)
                                       .Select(type => type.Assembly)
                                       .Distinct()
                                       .ToArray();
        }
    }
}