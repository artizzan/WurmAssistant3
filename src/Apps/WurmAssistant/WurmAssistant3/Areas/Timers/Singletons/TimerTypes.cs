using System;
using AldursLab.Essentials.Collections;
using AldursLab.WurmAssistant3.Areas.Timers.Contracts;
using AldursLab.WurmAssistant3.Areas.Timers.Transients.Timers.Alignment;
using AldursLab.WurmAssistant3.Areas.Timers.Transients.Timers.Custom;
using AldursLab.WurmAssistant3.Areas.Timers.Transients.Timers.JunkSale;
using AldursLab.WurmAssistant3.Areas.Timers.Transients.Timers.Meditation;
using AldursLab.WurmAssistant3.Areas.Timers.Transients.Timers.MeditPath;
using AldursLab.WurmAssistant3.Areas.Timers.Transients.Timers.Prayer;
using AldursLab.WurmAssistant3.Areas.Timers.Transients.Timers.Sermon;

namespace AldursLab.WurmAssistant3.Areas.Timers.Singletons
{
    public class TimerTypes
    {
        readonly BidirectionalMap<RuntimeTypeId, Type> map = new BidirectionalMap<RuntimeTypeId, Type>();

        public TimerTypes()
        {
            map.Add(RuntimeTypeId.Meditation, typeof(MeditationTimer));
            map.Add(RuntimeTypeId.MeditPath, typeof(MeditPathTimer));
            map.Add(RuntimeTypeId.Prayer, typeof(PrayerTimer));
            map.Add(RuntimeTypeId.Sermon, typeof(SermonTimer));
            map.Add(RuntimeTypeId.Alignment, typeof(AlignmentTimer));
            map.Add(RuntimeTypeId.JunkSale, typeof(JunkSaleTimer));
            map.Add(RuntimeTypeId.LegacyCustom, typeof(CustomTimer));
        }

        public RuntimeTypeId GetIdForType(Type type)
        {
            return map.GetByKey2(type);
        }

        public Type GetTypeForId(RuntimeTypeId id)
        {
            return map.GetByKey1(id);
        }
    }
}