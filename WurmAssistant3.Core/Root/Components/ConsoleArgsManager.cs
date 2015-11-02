using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Core.Root.Components
{
    public class ConsoleArgsManager
    {
        readonly string[] args;

        public ConsoleArgsManager([NotNull] string[] args)
        {
            if (args == null) throw new ArgumentNullException("args");
            this.args = args;
        }

        public bool WurmUnlimitedMode
        {
            get
            {
                return args.Length > 0 && args[0].Equals("-WurmUnlimited", StringComparison.InvariantCultureIgnoreCase);
            }
        }
    }
}
