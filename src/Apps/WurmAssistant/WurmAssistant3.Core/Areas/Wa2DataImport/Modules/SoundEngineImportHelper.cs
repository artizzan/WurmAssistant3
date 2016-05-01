using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AldursLab.WurmAssistant3.Core.Areas.SoundManager.Contracts;
using WurmAssistantDataTransfer.Dtos;

namespace AldursLab.WurmAssistant3.Core.Areas.Wa2DataImport.Modules
{
    public static class SoundEngineImportHelper
    {
        public static Guid MergeSoundAndGetId(ISoundManager soundManager, Sound sound)
        {
            ISoundResource result;
            if (sound.Id != null)
            {
                result = soundManager.GetSoundById(sound.Id.Value);
                return result.IsNull ? soundManager.AddSound(sound) : result.Id;
            }
            else
            {
                result = soundManager.GetFirstSoundByName(sound.Name);
                return result.IsNull ? soundManager.AddSoundAsNewId(sound) : result.Id;
            }
        }
    }
}
