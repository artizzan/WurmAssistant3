using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AldursLab.Essentials.Eventing
{
    public class EventArgs<T> : EventArgs
    {
        public T Arg { get; private set; }

        public EventArgs(T arg)
        {
            Arg = arg;
        }
    }
}
