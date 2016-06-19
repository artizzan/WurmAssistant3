using System;

namespace AldursLab.WurmAssistant3.Areas.Main.Services.NewsServices
{
    public class PatchReleaseNewsInstance : NewsInstance
    {
        public PatchReleaseNewsInstance()
        {
            Version = new Version(0,0,0,0);
            Path = string.Empty;
            VersionParsed = false;
            NewsUrl = @"https://dl.dropboxusercontent.com/u/74314315/pages/WurmAssistant/patch-release.html";
        }

        public override string VersionString => "Patch release";
    }
}