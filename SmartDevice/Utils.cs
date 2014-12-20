using System;
using System.IO;

namespace SmartDevice
{
    public class Utils
    {
        public static byte[] GetBytes(uint number)
        {
            byte[] result = BitConverter.GetBytes(number);
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(result);

            return result;
        }

        public static byte[] BlockingRead(Stream streamToRead, int size)
        {
            byte[] result = new byte[size];

            int messageSizeBytesMissing = size;
            while (messageSizeBytesMissing > 0)
            {
                int bytesRead = streamToRead.Read(result, size - messageSizeBytesMissing, messageSizeBytesMissing);

                if (bytesRead == 0)
                {
                    break;
                }
                else
                {
                    messageSizeBytesMissing = messageSizeBytesMissing - bytesRead;
                }
            }

            if (messageSizeBytesMissing != 0)
            {
                result = null;
            }

            return result;
        }
    }
}
