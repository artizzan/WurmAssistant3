using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AldursLab.WurmAssistant3.Core.Areas.SoundEngine.Contracts;

namespace AldursLab.WurmAssistant3.Core.Areas.SoundEngine.Modules
{
    class SoundEngine : ISoundEngine
    {
        public IPlayingSoundHandle PlayOneShot(string soundName)
        {
            return new PlayingSoundHandleNullObject();
        }

        public ChooseSoundResult ChooseSound()
        {
            //todo
            return new ChooseSoundResult(ActionResult.Cancel, new SoundResourceNullObject());
        }
    }

    class PlayingSoundHandleNullObject : IPlayingSoundHandle
    {
    }

    class SoundResourceNullObject : ISoundResource
    {
        public string Name { get { return string.Empty; } }
    }
}
