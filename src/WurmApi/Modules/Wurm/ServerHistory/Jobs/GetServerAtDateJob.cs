using System;

namespace AldursLab.WurmApi.Modules.Wurm.ServerHistory.Jobs
{
    class GetServerAtDateJob
    {
        public GetServerAtDateJob(CharacterName characterName, DateTime dateTime)
        {
            CharacterName = characterName;
            DateTime = dateTime;
        }

        public CharacterName CharacterName { get; private set; }
        public DateTime DateTime { get; private set; }
    }
}