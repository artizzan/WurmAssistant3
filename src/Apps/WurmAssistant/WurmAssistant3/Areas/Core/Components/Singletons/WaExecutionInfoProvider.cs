using System;
using AldursLab.WurmAssistant3.Areas.Config.Contracts;
using AldursLab.WurmAssistant3.Areas.Core.Contracts;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.Core.Components.Singletons
{
    class WaExecutionInfoProvider : IWaExecutionInfoProvider
    {
        readonly IWurmAssistantConfig wurmAssistantConfig;
        readonly IWaVersion waVersion;

        public WaExecutionInfoProvider([NotNull] IWurmAssistantConfig wurmAssistantConfig, [NotNull] IWaVersion waVersion)
        {
            if (wurmAssistantConfig == null) throw new ArgumentNullException("wurmAssistantConfig");
            if (waVersion == null) throw new ArgumentNullException("waVersion");
            this.wurmAssistantConfig = wurmAssistantConfig;
            this.waVersion = waVersion;
        }

        public string Get()
        {
            string s = string.Empty;
            if (waVersion.Known)
            {
                s += string.Format("{0}", waVersion.AsString());
            }
            else
            {
                s += "unknown version";
            }
            s += " P:" + wurmAssistantConfig.RunningPlatform;
            return s;
        }
    }
}