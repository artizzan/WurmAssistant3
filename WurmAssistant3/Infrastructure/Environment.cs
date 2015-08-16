using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AldursLab.WurmAssistant3.Core.Infrastructure;

namespace AldursLab.WurmAssistant3.Infrastructure
{
    public class Environment : IEnvironment
    {
        bool closing;
        readonly object locker = new object();

        public bool Closing
        {
            get
            {
                lock (locker)
                {
                    return closing;
                }
            }
            set
            {
                lock (locker)
                {
                    closing = value;
                }
            }
        }

        public void RequestRestart()
        {
            throw new NotSupportedException("WPF does not support automatic application restart");
        }
    }
}
