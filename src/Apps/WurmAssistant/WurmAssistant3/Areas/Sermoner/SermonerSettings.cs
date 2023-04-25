using AldursLab.PersistentObjects;
using Newtonsoft.Json;

namespace AldursLab.WurmAssistant3.Areas.Sermoner
{
    [KernelBind(BindingHint.Singleton), PersistentObject("SermonerFeature_Settings")]
    public class SermonerSettings : PersistentObjectBase
    {
        [JsonProperty]
        public int _omitOldPreacherTime;

        public int OmitOldPreacherTime
        {
            get { return _omitOldPreacherTime; }
            set { _omitOldPreacherTime = value; FlagAsChanged(); }
        }

        byte[] sermonerViewState = new byte[0];
        public byte[] SermonerViewState
        {
            get { return sermonerViewState; }
            set { sermonerViewState = value; FlagAsChanged(); }
        }
    }
}
