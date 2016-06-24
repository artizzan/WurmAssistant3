using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace AldursLab.WurmAssistant3.Areas.SamplePlugin.ViewModels
{
    [KernelBind(BindingHint.Transient)]
    public class SomethingViewModel : PropertyChangedBase
    {
        string someDataBoundString;

        public string SomeDataBoundString
        {
            get { return someDataBoundString; }
            set
            {
                if (value == someDataBoundString) return;
                someDataBoundString = value;
                NotifyOfPropertyChange();
            }
        }
    }
}
