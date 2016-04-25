namespace AldursLab.WurmApi.Modules.Wurm.ServerHistory.Jobs
{
    class GetCurrentServerJob
    {
        public GetCurrentServerJob(CharacterName characterName)
        {
            CharacterName = characterName;
        }

        public CharacterName CharacterName { get; private set; }
    }
}