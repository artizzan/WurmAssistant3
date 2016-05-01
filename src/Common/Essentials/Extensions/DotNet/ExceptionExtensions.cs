using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AldursLab.Essentials.Extensions.DotNet
{
    public static class ExceptionExtensions
    {
        public static string ToStringMessagesOnly(this Exception exception)
        {
            string message = exception.Message;
            if (exception.InnerException != null)
            {
                return message + " > " + ToStringMessagesOnly(exception.InnerException);
            }
            else return message;
        }
    }
}
