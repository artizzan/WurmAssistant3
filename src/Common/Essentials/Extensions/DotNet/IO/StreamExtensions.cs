using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AldursLab.Essentials.Extensions.DotNet.IO
{
    public static class StreamExtensions
    {
        /// <summary>
        /// Reads entire stream into byte array.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static byte[] ReadToByteArray(this Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
    }
}
