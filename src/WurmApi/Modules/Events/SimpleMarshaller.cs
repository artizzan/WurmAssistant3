using System;

namespace AldursLab.WurmApi.Modules.Events
{
    class SimpleMarshaller : IWurmApiEventMarshaller
    {
        public void Marshal(Action action)
        {
            action();
        }
    }
}
