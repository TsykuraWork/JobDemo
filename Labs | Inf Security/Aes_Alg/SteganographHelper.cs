using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab6
{
    public static class SteganographHelper
    {
        public static BitArray ByteToBits(byte src)
        {
            return new BitArray(new byte[] { src });
        }

        public static BitArray ByteToBits(byte[] src)
        {
            return new BitArray(src);
        }

        public static byte BitsToByte(BitArray byteValue)
        {
            if (byteValue.Count != 8)
                throw new ArgumentException("Количество элементов в массиве не соответствует 8.");
            byte[] ret = new byte[1];
            byteValue.CopyTo(ret, 0);
            return ret[0];
        }

        public static int BitsToInt32(BitArray intValue)
        {
            if (intValue.Count != 32)
                throw new ArgumentException("Количество элементов в массиве не соответствует 32.");

            byte[] byteDataLength = new byte[4];
            int bitCounter = 0;
            for (int i = 0; i < 4; i++)
            {
                BitArray bitArray = new BitArray(8);
                for (int j = 0; j < 8; j++)
                {
                    bitArray[j] = intValue[bitCounter];
                    bitCounter++;
                }
                byteDataLength[i] = BitsToByte(bitArray);
            }

            return BitConverter.ToInt32(byteDataLength, 0);
        }

        public static byte[] BitsToByteArray(BitArray bitBytes)
        {
            if (bitBytes.Count % 8 != 0)
                throw new ArgumentException("Количество элементов в массиве должно делиться нацело на 8.");

            byte[] retBytes = new byte[bitBytes.Length / 8];
            int byteCounter = 0;

            for (int i = 0; i < bitBytes.Length; i += 8)
            {
                BitArray byteVal = new BitArray(8);

                for (int j = 0; j < byteVal.Length; j++)
                    byteVal[j] = bitBytes[i + j];

                retBytes[byteCounter] = BitsToByte(byteVal);
                byteCounter++;
            }

            return retBytes;
        }
    }
}
