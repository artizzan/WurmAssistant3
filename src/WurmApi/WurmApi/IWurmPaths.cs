namespace AldursLab.WurmApi
{
    public interface IWurmPaths
    {
        string LogsDirName { get; }
        string OldWuLogsDirName { get; }

        string ConfigsDirFullPath { get; }
        string CharactersDirFullPath { get; }

        string GetSkillDumpsFullPathForCharacter(CharacterName characterName);
        string GetLogsDirFullPathForCharacter(CharacterName characterName);
        string GetScreenshotsDirFullPathForCharacter(CharacterName characterName);
        string GetOldWuLogsDirFullPathForCharacter(CharacterName characterName);
    }
}