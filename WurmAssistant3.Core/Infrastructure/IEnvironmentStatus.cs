using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AldursLab.WurmAssistant3.Core.Infrastructure
{
    public interface IEnvironmentStatus
    {
        bool Closing { get; }
    }
}
