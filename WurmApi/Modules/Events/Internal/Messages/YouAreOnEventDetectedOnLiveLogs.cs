using System;
using JetBrains.Annotations;

namespace AldursLab.WurmApi.Modules.Events.Internal.Messages
{
    /// <summary>
    /// This event happens when player logs in or travels between server (which is technically a login to another server as well)
    /// </summary>
    class YouAreOnEventDetectedOnLiveLogs : Message
    {
        public YouAreOnEventDetectedOnLiveLogs([NotNull] ServerName serverName, [NotNull] CharacterName characterName,
            bool currentServerNameChanged)
        {
            if (serverName == null) throw new ArgumentNullException(nameof(serverName));
            if (characterName == null) throw new ArgumentNullException(nameof(characterName));
            ServerName = serverName;
            CharacterName = characterName;
            CurrentServerNameChanged = currentServerNameChanged;
        }

        public ServerName ServerName { get; }

        public CharacterName CharacterName { get; }

        public bool CurrentServerNameChanged { get; }
    }
}
