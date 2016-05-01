using System;
using System.Linq;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant.Launcher.Modules
{
    public class ArgsManager
    {
        readonly string[] args;

        const string BuildCodeParam = "-buildcode";
        const string BuildNumberParam = "-buildnumber";

        const string WurmUnlimitedFlag = "-wurmunlimited";
        const string ShowConfigWindowFlag = "-showconfig";
        const string RelativeDataDirFlag = "-relativedatadir";

        public ArgsManager([NotNull] string[] args)
        {
            if (args == null) throw new ArgumentNullException("args");
            this.args = args;

            BuildCode = string.Empty;

            ParseArgs();
        }

        public bool HasBuildCode { get { return !string.IsNullOrWhiteSpace(BuildCode); } }

        public string BuildCode { get; private set; }

        public bool WurmUnlimitedMode { get; private set; }

        public bool NoArgs { get; private set; }

        public bool UseRelativeWaDataDir { get; private set; }

        public int SpecificBuildNumber { get; private set; }

        public bool HasSpecificBuildNumber { get { return SpecificBuildNumber > 0; } }

        public bool ShowConfigWindow { get; private set; }

        void ParseArgs()
        {
            if (!args.Any())
            {
                NoArgs = true;
                ShowConfigWindow = true;
                return;
            }

            for (int i = 0; i < args.Length; i++)
            {
                var arg = args[i];

                if (ParamMatch(arg, BuildCodeParam))
                {
                    var value = GetValueForArg(args, i);
                    BuildCode = value;
                    i++;
                }
                else if (ParamMatch(arg, WurmUnlimitedFlag))
                {
                    WurmUnlimitedMode = true;
                }
                else if (ParamMatch(arg, RelativeDataDirFlag))
                {
                    UseRelativeWaDataDir = true;
                }
                else if (ParamMatch(arg, BuildNumberParam))
                {
                    var value = GetValueForArg(args, i);
                    SpecificBuildNumber = int.Parse(value);
                    i++;
                }
            }
        }

        bool ParamMatch(string arg, string param)
        {
            if (arg == null) return false;
            if (param == null) return false;
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