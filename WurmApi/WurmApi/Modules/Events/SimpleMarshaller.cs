using System;

namespace AldurSoft.WurmApi.Modules.Events
{
    class SimpleMarshaller : IEventMarshaller
    {
        public void Marshal(Action action)
        {
            action();
        }
    }
}
