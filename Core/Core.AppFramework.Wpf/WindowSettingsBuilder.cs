using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Core.AppFramework.Wpf
{
    public class WindowSettingsBuilder
    {
        public int? WidthPx { get; set; }
        public int? HeightPx { get; set; }
        public WindowStartupLocation? WindowStartupLocation { get; set; }
        public string WindowTitle { get; set; }

        public IDictionary<string, object> ToDictionary()
        {
            var result = new Dictionary<string, object>();

            if (WidthPx.HasValue)
            {
                result.Add("Width", WidthPx.Value);
            }

            if (HeightPx.HasValue)
            {
                result.Add("Height", HeightPx.Value);
            }

            if (WindowStartupLocation.HasValue)
            {
                result.Add("WindowStartupLocation", WindowStartupLocation.Value);
            }

            if (WindowTitle != null)
            {
                result.Add("Title", WindowTitle);
            }

            return result;
        }
    }
}
