using System.Threading;
using JetBrains.Annotations;
using Ninject;

namespace AldursLab.WurmAssistant3.Areas.TestArea1
{
    [UsedImplicitly]
    public class TestArea1Config : AreaConfig
    {
        public static bool HasBeenRun { get; private set; }

        public override void Configure(IKernel kernel)
        {
            HasBeenRun = true;
        }
    }
}
