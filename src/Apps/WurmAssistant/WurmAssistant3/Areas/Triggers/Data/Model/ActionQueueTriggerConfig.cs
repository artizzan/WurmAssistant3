using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using AldursLab.WurmAssistant3.Areas.Triggers.ActionQueueParsing;
using AldursLab.WurmAssistant3.Utils.Collections;
using Caliburn.Micro;

namespace AldursLab.WurmAssistant3.Areas.Triggers.Data.Model
{
    [DataContract]
    public class ActionQueueTriggerConfig : PropertyChangedBase
    {
        MemberChangeNotifyingDictionary<Guid, Condition> conditions = new MemberChangeNotifyingDictionary<Guid, Condition>();

        [DataMember]
        public MemberChangeNotifyingDictionary<Guid, Condition> Conditions
        {
            get { return conditions; }
            set
            {
                if (Equals(value, conditions)) return;
                conditions = value;
                NotifyOfPropertyChange();
            }
        }
    }
}