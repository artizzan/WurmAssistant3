using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace AldurSoft.WurmApi.Modules.Events.Internal.Messages
{
    class CharacterLogFilesAddedOrRemoved : Message
    {
        public CharacterLogFilesAddedOrRemoved([NotNull] CharacterName characterName)
        {
            if (characterName == null) throw new ArgumentNullException("characterName");
            CharacterName = characterName;
        }

        public CharacterName CharacterName { get; private set; }
    }
}
