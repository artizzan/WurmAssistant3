using System;
using JetBrains.Annotations;

namespace AldursLab.WurmApi.Modules.Events.Internal.Messages
{
    class CharacterLogFilesAddedOrRemoved : Message
    {
        public CharacterLogFilesAddedOrRemoved([NotNull] CharacterName characterName)
        {
            if (characterName == null) throw new ArgumentNullException(nameof(characterName));
            CharacterName = characterName;
        }

        public CharacterName CharacterName { get; private set; }
    }
}
