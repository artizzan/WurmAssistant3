using System;

namespace AldursLab.WurmAssistant3.Core.Areas.Granger.Legacy.LogFeedManager
{
    [Obsolete]
    /// <summary>
    /// Moved to GrangerHelpers
    /// </summary>
    public static class WurmEventParser
    {
        public static HorseTrait[] GetTraitsFromLine(string line)
        {
            return GrangerHelpers.GetTraitsFromLine(line);
        }
    }
}
