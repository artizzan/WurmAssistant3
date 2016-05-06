using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AldursLab.Testing
{
    public static class TeardownHelper
    {
        public static void DisposeAll(params IDisposable[] disposables)
        {
            if (disposables == null) throw new ArgumentNullException(nameof(disposables));
            List<Exception> exceptions = new List<Exception>();
            foreach (var disposable in disposables)
            {
                try
                {
                    disposable.Dispose();
                }
                catch (Exception exception)
                {
                    exceptions.Add(exception);
                }
            }
            if (exceptions.Any())
            {
                throw new AggregateException("At least one dispose call resulted in exception", exceptions);
            }
        }
    }
}
