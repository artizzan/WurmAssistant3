using System.Threading;
using Ninject;

namespace AldursLab.WurmAssistant3.Areas.TestArea1
{
    public class AreaConfiguration : IAreaConfiguration
    {
        public static bool HasBeenRun { get; private set; }

        public void Configure(IKernel kernel)
        {
            HasBeenRun = true;
        }
    }
}
