using System;
using AldursLab.WurmApi.Modules.Wurm.Servers.WurmServersModel;
using JetBrains.Annotations;

namespace AldursLab.WurmApi.Modules.Wurm.Servers
{
    class LiveLogsDataForCharacter
    {
        public LiveLogsDataForCharacter(CharacterName character, [CanBeNull] ServerDateStamped wurmDateTime, 
            [CanBeNull] ServerUptimeStamped uptime)
        {
            if (character == null) throw new ArgumentNullException(nameof(character));
            Character = character;
            WurmDateTime = wurmDateTime;
            Uptime = uptime;
        }

        public CharacterName Character { get; }

        public ServerDateStamped WurmDateTime { get; private set; }

        public ServerUptimeStamped Uptime { get; private set; }
    }
}