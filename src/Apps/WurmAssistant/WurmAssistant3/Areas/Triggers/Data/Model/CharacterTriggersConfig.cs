using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using AldursLab.WurmAssistant3.Utils.Collections;
using Caliburn.Micro;

namespace AldursLab.WurmAssistant3.Areas.Triggers.Data.Model
{
    [DataContract]
    public class CharacterTriggersConfig : PropertyChangedBase
    {
        bool muted = false;
        byte[] triggerListState = new byte[0];
        MemberChangeNotifyingDictionary<Guid, TriggerEntity> triggerEntities = new MemberChangeNotifyingDictionary<Guid, TriggerEntity>();

        [DataMember]
        public bool Muted
        {
            get { return muted; }
            set
            {
                if (value == muted) return;
                muted = value;
                NotifyOfPropertyChange();
            }
        }

        [DataMember]
        public byte[] TriggerListState
        {
            get { return triggerListState; }
            set
            {
                if (Equals(value, triggerListState)) return;
                triggerListState = value;
                NotifyOfPropertyChange();
            }
        }

        [DataMember]
        public MemberChangeNotifyingDictionary<Guid, TriggerEntity> TriggerEntities
        {
            get { return triggerEntities; }
            set
            {
                if (Equals(value, triggerEntities)) return;
                triggerEntities = value;
                NotifyOfPropertyChange();
            }
        }
    }
}
