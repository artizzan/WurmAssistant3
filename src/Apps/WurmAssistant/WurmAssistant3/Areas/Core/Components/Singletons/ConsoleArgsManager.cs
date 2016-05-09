using System;
using System.Linq;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.Core.Components.Singletons
{
    public class ConsoleArgsManager
    {
        const string WurmUnlimitedFlag = "-wurmunlimited";
        const string RelativeDataDirFlag = "-relativedatadir";

        readonly string[] args;

        bool wurmUnlimitedMode = false;
        bool useRelativeDataDir = false;

        public ConsoleArgsManager([NotNull] string[] args)
        {
            if (args == null) throw new ArgumentNullException("args");
            this.args = args;

            ParseArgs();
        }

        void ParseArgs()
        {
            if (!args.Any())
            {
                return;
            }

            for (int i = 0; i < args.Length; i++)
            {
                var arg = args[i];

                if (ParamMatch(arg, RelativeDataDirFlag))
                {
                    useRelativeDataDir = true;
                }
                else if (ParamMatch(arg, WurmUnlimitedFlag))
                {
                    wurmUnlimitedMode = true;
                }
            }
        }

        public string GetRawArgs()
        {
            return string.Join(" ", args);
        }

        public bool WurmUnlimitedMode
        {
            get { return wurmUnlimitedMode; }
        }

        public bool UseRelativeDataDir
        {
            get { return useRelativeDataDir; }
        }

        bool ParamMatch(string arg, string param)
        {
            if (arg == null)
                return false;
            if (param == null)
                return false;
            return arg.Equals(param, StringComparison.InvariantCultureIgnoreCase);
        }

        string GetValueForArg(string[] args, int argIndex)
        {
            var nextIndex = argIndex + 1;
            if (args.Length < nextIndex)
            {
                throw new ApplicationException("Expected argument value, found end of args array");
            }
            else if (args[nextIndex].StartsWith("-"))
            {
                throw new ApplicationException("Expected argument value, found param");
            }
            else
            {
                return args[nextIndex];
            }
        }
    }
}
