using System;
using AldurSoft.WurmApi.Modules.DataContext.DataModel.WurmServersModel;

namespace AldurSoft.WurmApi.Modules.Wurm.Servers
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