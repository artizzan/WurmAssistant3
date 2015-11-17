using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AldursLab.WurmAssistant.Shared.Dtos
{
    public class Package
    {
        public Guid WurmAssistantPackageId { get; set; }
        public string BuildCode { get; set; }
        public string BuildNumber { get; set; }
    }
}
