using System;
using AldurSoft.WurmApi.Persistence.DataModel.WurmServersModel;
using AldurSoft.WurmApi.Wurm.Characters;

namespace AldurSoft.WurmApi.Wurm.Servers.WurmServersModule
{
    class LiveLogsDataForCharacter
    {
        private readonly CharacterName character;

        public LiveLogsDataForCharacter(CharacterName character)
        {
            if (character == null) throw new ArgumentNullException("character");
            this.character = character;
        }

        public CharacterName Character
        {
            get { return character; }
        }

        public ServerDateStamped WurmDateTime { get; set; }

        public ServerUptimeStamped Uptime { get; set; }
    }
}