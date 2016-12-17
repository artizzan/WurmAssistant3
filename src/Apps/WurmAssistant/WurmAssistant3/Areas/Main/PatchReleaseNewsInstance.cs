using System;

namespace AldursLab.WurmAssistant3.Areas.Main
{
    public class PatchReleaseNewsInstance : NewsInstance
    {
        public PatchReleaseNewsInstance()
        {
            Version = new Version(0,0,0,0);
            Path = string.Empty;
            VersionParsed = false;
            NewsUrl = @"http://content.mdsolver.net/wurmassistant/news/patch-release.html";
        }

        public override string VersionString => "Patch release";
    }
}