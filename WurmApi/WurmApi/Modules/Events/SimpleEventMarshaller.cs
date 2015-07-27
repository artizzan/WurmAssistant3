using System;

namespace AldurSoft.WurmApi.Modules.Events
{
    class SimpleEventMarshaller : IEventMarshaller
    {
        public void Marshal(Action action)
        {
            action();
        }
    }
}
