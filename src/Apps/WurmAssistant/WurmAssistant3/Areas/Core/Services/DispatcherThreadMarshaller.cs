using System;
using System.Windows;
using AldursLab.Essentials.Eventing;
using AldursLab.WurmApi;

namespace AldursLab.WurmAssistant3.Areas.Core.Services
{
    [KernelHint(BindingHint.Singleton)]
    class DispatcherThreadMarshaller : IThreadMarshaller, IWurmApiEventMarshaller
    {
        void IThreadMarshaller.Marshal(Action action)
        {
            InvokeOnUiThread(action);
        }

        void IWurmApiEventMarshaller.Marshal(Action action)
        {
            InvokeOnUiThread(action);
        }

        static void InvokeOnUiThread(Action action)
        {
            if (Application.Current.Dispatcher.CheckAccess())
            {
                action.Invoke();
            }
            else
            {
                Application.Current.Dispatcher.Invoke(action);
            }
        }
    }
}