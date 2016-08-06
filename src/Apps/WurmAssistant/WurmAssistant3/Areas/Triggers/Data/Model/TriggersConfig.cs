using System.Collections.Generic;
using System.Runtime.Serialization;
using AldursLab.WurmAssistant3.Tests.Unit.Utils.Collections;
using Caliburn.Micro;

namespace AldursLab.WurmAssistant3.Areas.Triggers.Data.Model
{
    [DataContract]
    public class TriggersConfig : PropertyChangedBase
    {
        MemberChangeNotifyingHashset<string> activeCharacterNames = new MemberChangeNotifyingHashset<string>();

        [DataMember]
        public MemberChangeNotifyingHashset<string> ActiveCharacterNames
        {
            get { return activeCharacterNames; }
            set
            {
                if (Equals(value, activeCharacterNames)) return;
                activeCharacterNames = value;
                NotifyOfPropertyChange();
            }
        }
    }
}