using System;

namespace AldurSoft.WurmApi.Modules.Events
{
    class SimpleMarshaller : IPublicEventMarshaller
    {
        public void Marshal(Action action)
        {
            action();
        }
    }
}
