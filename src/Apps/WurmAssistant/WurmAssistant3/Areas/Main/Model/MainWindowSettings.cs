using System.Runtime.Serialization;
using Caliburn.Micro;
using Newtonsoft.Json;

namespace AldursLab.WurmAssistant3.Areas.Main.Model
{
    [DataContract]
    public class MainWindowSettings : PropertyChangedBase
    {
        int width = 622;
        int height = 452;

        [DataMember]
        public int Width
        {
            get
            {
                return width;
            }
            set
            {
                if (value.Equals(width)) return;
                width = value;
                NotifyOfPropertyChange();
            }
        }

        [DataMember]
        public int Height
        {
            get
            {
                return height;
            }
            set
            {
                if (value.Equals(height)) return;
                height = value;
                NotifyOfPropertyChange();
            }
        }
    }
}
