using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace AldursLab.WurmAssistant3.Areas.Main.Data.Model
{
    [DataContract]
    public class News : PropertyChangedBase
    {
        Version lastShownNewsVersion = new Version(0,0,0,0);

        [DataMember]
        public Version LastShownNewsVersion
        {
            get { return lastShownNewsVersion; }
            set
            {
                if (Equals(value, lastShownNewsVersion)) return;
                lastShownNewsVersion = value;
                NotifyOfPropertyChange();
            }
        }
    }
}
