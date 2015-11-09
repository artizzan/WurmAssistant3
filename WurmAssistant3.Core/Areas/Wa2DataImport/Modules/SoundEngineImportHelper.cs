using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AldursLab.WurmAssistant3.Core.Areas.SoundEngine.Contracts;
using WurmAssistantDataTransfer.Dtos;

namespace AldursLab.WurmAssistant3.Core.Areas.Wa2DataImport.Modules
{
    public static class SoundEngineImportHelper
    {
        public static Guid MergeSoundAndGetId(ISoundEngine soundEngine, Sound sound)
        {
            ISoundResource result;
            if (sound.Id != null)
            {
                result = soundEngine.GetSoundById(sound.Id.Value);
                return result.IsNull ? soundEngine.AddSound(sound) : result.Id;
            }
            else
            {
                result = soundEngine.GetFirstSoundByName(sound.Name);
                return result.IsNull ? soundEngine.AddSoundAsNewId(sound) : result.Id;
            }
        }
    }
}
