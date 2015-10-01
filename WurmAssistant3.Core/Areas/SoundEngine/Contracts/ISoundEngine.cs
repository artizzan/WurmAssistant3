using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AldursLab.WurmAssistant3.Core.Areas.SoundEngine.Contracts
{
    /// <summary>
    /// An engine that can be used to play sounds and manage sounds library
    /// </summary>
    public interface ISoundEngine
    {
        /// <summary>
        /// Immediatelly plays a sound with given name, if sound exists.
        /// Names are case-insensitive.
        /// Returns a handle that enables control over playing sound. 
        /// Handle is never null, if sound was not found, methods on handle do nothing.
        /// </summary>
        /// <param name="soundName">case-insensitive</param>
        IPlayingSoundHandle PlayOneShot(string soundName);

        ChooseSoundResult ChooseSound();
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
