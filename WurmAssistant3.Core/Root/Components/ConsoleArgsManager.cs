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

        readonly bool wurmUnlimitedMode;
        readonly bool invalidCmdLineArgs;

        public ConsoleArgsManager([NotNull] string[] args)
        {
            if (args == null) throw new ArgumentNullException("args");
            this.args = args;

            if (args.Length > 0)
            {
                if (args[0].Equals("-WurmUnlimited", StringComparison.InvariantCultureIgnoreCase))
                {
                    wurmUnlimitedMode = true;
                }
                else
                {
                    invalidCmdLineArgs = true;
                }
            }
        }

        public string GetRawArgs()
        {
            return string.Join(" ", args);
        }

        public bool InvalidCmdLineArgs
        {
            get
            {
                return invalidCmdLineArgs;
            }
        }

        public bool WurmUnlimitedMode
        {
            get
            {
                return wurmUnlimitedMode;
            }
        }
    }
}
