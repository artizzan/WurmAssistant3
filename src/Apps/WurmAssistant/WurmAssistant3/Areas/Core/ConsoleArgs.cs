using System;
using System.Linq;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.Core
{
    public class ConsoleArgs : IConsoleArgs
    {
        readonly string executingAssemblyNameWithoutExtension;

        const string WurmUnlimitedFlag = "-wurmunlimited";
        const string RelativeDataDirFlag = "-relativedatadir";

        readonly string[] args;

        bool wurmUnlimitedMode = false;
        bool useRelativeDataDir = false;

        public ConsoleArgs([NotNull] string executingAssemblyNameWithoutExtension) //todo: use IEnvironment?
        {
            if (executingAssemblyNameWithoutExtension == null) throw new ArgumentNullException(nameof(executingAssemblyNameWithoutExtension));

            this.executingAssemblyNameWithoutExtension = executingAssemblyNameWithoutExtension;
            this.args = Environment.GetCommandLineArgs();

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
                var arg = args[i] ?? string.Empty;

                if (ParamMatch(arg, RelativeDataDirFlag))
                {
                    useRelativeDataDir = true;
                }
                else if (ParamMatch(arg, WurmUnlimitedFlag))
                {
                    wurmUnlimitedMode = true;
                }
                else
                {
                    if (!CommonArgument(arg))
                    {
                        throw new WurmAssistantException("Unknown console argument: " + arg);
                    }
                }
            }
        }

        public string GetRawArgs()
        {
            return string.Join(" ", args);
        }

        public bool WurmUnlimitedMode => wurmUnlimitedMode;

        public bool UseRelativeDataDir => useRelativeDataDir;

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

        bool CommonArgument(string arg)
        {
            return arg.EndsWith(executingAssemblyNameWithoutExtension + ".vshost.exe", StringComparison.OrdinalIgnoreCase)
                   || arg.EndsWith(executingAssemblyNameWithoutExtension + "exe", StringComparison.OrdinalIgnoreCase);
        }
    }
}
