using System;
using AldurSoft.WurmApi.Modules.DataContext.DataModel.WurmServersModel;
using JetBrains.Annotations;

namespace AldurSoft.WurmApi.Modules.Wurm.Servers
{
    class LiveLogsDataForCharacter
    {
        private readonly CharacterName character;

        public LiveLogsDataForCharacter(CharacterName character, [CanBeNull] ServerDateStamped wurmDateTime, 
            [CanBeNull] ServerUptimeStamped uptime)
        {
            if (character == null) throw new ArgumentNullException("character");
            this.character = character;
            WurmDateTime = wurmDateTime;
            Uptime = uptime;
        }

        public CharacterName Character
        {
            get { return character; }
        }

        public ServerDateStamped WurmDateTime { get; private set; }

        public ServerUptimeStamped Uptime { get; private set; }
    }
}