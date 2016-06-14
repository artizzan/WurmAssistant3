using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Ninject;

namespace AldursLab.WurmAssistant3.Areas.SamplePlugin
{
    // This class is used automatically by Wurm Assistant to setup your plugin.

    [UsedImplicitly]
    public class SamplePluginAreaConfiguration : AreaConfig
    {
        public override void Configure(IKernel kernel)
        {
            // If conventions work for you, this method can be left empty.

            // If you don't like conventions or they are too simple for your needs,
            // you can setup all your bindings here, as you see fit.
        }
    }
}
