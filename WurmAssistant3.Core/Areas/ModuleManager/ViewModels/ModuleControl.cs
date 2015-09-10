using System;

namespace AldursLab.WurmAssistant3.Core.Areas.ModuleManager.ViewModels
{
    public class ModuleControl
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