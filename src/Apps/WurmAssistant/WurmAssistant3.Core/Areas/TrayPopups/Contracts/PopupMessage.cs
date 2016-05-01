using System;
using Newtonsoft.Json;

namespace AldursLab.WurmAssistant3.Core.Areas.TrayPopups.Contracts
{
    public class PopupMessage
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public int DurationMillis { get; set; }
    }
}
