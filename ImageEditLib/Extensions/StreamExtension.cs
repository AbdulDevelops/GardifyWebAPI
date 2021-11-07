using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ImageEditLib.Extensions
{
    public static class StreamExtension
    {
        /// <summary>
        /// Translates the Stream into a byte[]
        /// </summary>
        /// <param name="input">Stream on which the operation should be operated</param>
        /// <returns>byte[] representation of the Stream</returns>
        public static byte[] ReadFully(this Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
    }
}
