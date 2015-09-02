using System;

namespace AldursLab.WurmAssistant3.Core.ViewModels.ModuleManagement
{
    public class ModuleControlViewModel
    {
        public string Name { get; set; }
        public Action OpenAction { get; set; }

        public void Open()
        {
            var oa = OpenAction;
            if (oa != null)
            {
                OpenAction();
            }
        }
    }
}