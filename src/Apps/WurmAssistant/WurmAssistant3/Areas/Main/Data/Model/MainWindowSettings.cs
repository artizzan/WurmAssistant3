using System.Runtime.Serialization;
using Caliburn.Micro;

namespace AldursLab.WurmAssistant3.Areas.Main.Data.Model
{
    [DataContract]
    public class MainWindowSettings : PropertyChangedBase
    {
        double width = 622;
        double height = 452;

        [DataMember]
        public double Width
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
        public double Height
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
