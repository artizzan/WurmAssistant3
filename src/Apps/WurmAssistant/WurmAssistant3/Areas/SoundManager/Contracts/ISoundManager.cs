using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using WurmAssistantDataTransfer.Dtos;

namespace AldursLab.WurmAssistant3.Areas.SoundManager.Contracts
{
    public interface ISoundManager
    {
        IPlayingSoundHandle PlayOneShot(Guid soundId);

        IPlayingSoundHandle PlayOneShot(ISoundResource soundResource);

        /// <summary>
        /// Shows a window dialog with available sounds.
        /// </summary>
        ChooseSoundResult ChooseSound();

        /// <summary>
        /// Shows a window dialog with available sounds, centered at parentForm.
        /// </summary>
        ChooseSoundResult ChooseSound(Form parentForm);

        /// <summary>
        /// Shows sound manager GUI
        /// </summary>
        void ShowSoundManager();

        void StopAllSounds();

        float GlobalVolume { get; set; }

        ISoundResource GetSoundById(Guid soundId);

        ISoundResource GetFirstSoundByName(string name);

        /// <exception cref="ArgumentException">Sound with this Id already exists.</exception>
        Guid AddSound(Sound sound);

        Guid AddSoundAsNewId(Sound sound);
        void HideGui();
        Task ImportDataFromWa2Async(WurmAssistantDto dto);
    }

    public class ChooseSoundResult
    {
        public ActionResult ActionResult { get; private set; }

        public ISoundResource SoundResource { get; private set; }

        public ChooseSoundResult(ActionResult actionResult, ISoundResource soundResource)
        {
            ActionResult = actionResult;
            SoundResource = soundResource;
        }
    }

    public enum ActionResult
    {
        Cancel = 0,
        Ok
    }
}
