using System;

namespace AldursLab.WurmAssistant3.Areas.Main.Services.NewsServices
{
    public abstract class NewsInstance
    {
        public Version Version { get; protected set; }
        public string Path { get; protected set; }
        public bool VersionParsed { get; protected set; }
        public string NewsUrl { get; protected set; } = string.Empty;

        public virtual string VersionString => $"{Version.Major}.{Version.Minor}";

        protected NewsInstance()
        {
        }
    }
}