using System;
using System.Collections;
using System.Drawing;
using System.Collections.Generic;

namespace lab6
{
    public static class Steganographer
    {
        public static Bitmap Introduce(Bitmap bmp, byte[] data)
        {
            BitArray bitData = SteganographHelper.ByteToBits(data);

            if (bitData.Length > (bmp.Width * bmp.Height - 12) * 3)
                throw new ArgumentException("Недостаточно места для внедрения файла такого размера.");

            int bitDataCounter = 0;

            WriteDataLength(bmp, bitData.Length);

            for (int i = 0; i < bmp.Width && bitDataCounter < bitData.Length; i++)
            {
                for (int j = 0; j < bmp.Height && bitDataCounter < bitData.Length; j++)
                {
                    if (i == 0 && j < 12)
                        continue;

                    Color oldColor = bmp.GetPixel(i, j);

                    BitArray r = SteganographHelper.ByteToBits(oldColor.R);
                    BitArray g = SteganographHelper.ByteToBits(oldColor.G);
                    BitArray b = SteganographHelper.ByteToBits(oldColor.B);

                    int counter = 0;
                    while (bitDataCounter < bitData.Length && counter < 1)
                    {
                        r[counter] = bitData[bitDataCounter];
                        bitDataCounter++;
                        counter++;
                    }

                    counter = 0;
                    while (bitDataCounter < bitData.Length && counter < 1)
                    {
                        g[counter] = bitData[bitDataCounter];
                        bitDataCounter++;
                        counter++;
                    }

                    counter = 0;
                    while (bitDataCounter < bitData.Length && counter < 1)
                    {
                        b[counter] = bitData[bitDataCounter];
                        bitDataCounter++;
                        counter++;
                    }

                    Color newColor = Color.FromArgb(
                        SteganographHelper.BitsToByte(r),
                        SteganographHelper.BitsToByte(g),
                        SteganographHelper.BitsToByte(b)
                        );
                    bmp.SetPixel(i, j, newColor);
                }
            }
            return bmp;
        }

        public static byte[] Extract(Bitmap bmp)
        {
            try
            {
                BitArray bitData = new BitArray(ReadDataLength(bmp));
                int bitDataCounter = 0;

                for (int i = 0; i < bmp.Width && bitDataCounter < bitData.Length; i++)
                {
                    for (int j = 0; j < bmp.Height && bitDataCounter < bitData.Length; j++)
                    {
                        if (i == 0 && j < 12)
                            continue;

                        Color color = bmp.GetPixel(i, j);

                        BitArray r = SteganographHelper.ByteToBits(color.R);
                        BitArray g = SteganographHelper.ByteToBits(color.G);
                        BitArray b = SteganographHelper.ByteToBits(color.B);

                        int counter = 0;
                        while (bitDataCounter < bitData.Length && counter < 1)
                        {
                            bitData[bitDataCounter] = r[counter];
                            bitDataCounter++;
                            counter++;
                        }

                        counter = 0;
                        while (bitDataCounter < bitData.Length && counter < 1)
                        {
                            bitData[bitDataCounter] = g[counter];
                            bitDataCounter++;
                            counter++;
                        }

                        counter = 0;
                        while (bitDataCounter < bitData.Length && counter < 1)
                        {
                            bitData[bitDataCounter] = b[counter];
                            bitDataCounter++;
                            counter++;
                        }
                    }
                }

                return SteganographHelper.BitsToByteArray(bitData);
            }
            catch (Exception)
            {
                throw new Exception("При извлечении произошла ошибка." +
                    " Возможно файл повреждён или в нём отсутствует" +
                    " внедрённая информация.");
            }
        }

        private static void WriteDataLength(Bitmap bmp, int bitDataLength)
        {
            BitArray dataLength = SteganographHelper.ByteToBits(
                BitConverter.GetBytes(bitDataLength));
            int bitCounter = 0;

            for (int i = 0; i < 12; i++)
            {
                Color color = bmp.GetPixel(0, i);
                BitArray r = SteganographHelper.ByteToBits(color.R);
                BitArray g = SteganographHelper.ByteToBits(color.G);
                BitArray b = SteganographHelper.ByteToBits(color.B);

                int counter = 0;
                while (counter < 1 && bitCounter < dataLength.Length)
                {
                    r[counter] = dataLength[bitCounter];
                    counter++;
                    bitCounter++;
                }

                counter = 0;
                while (counter < 1 && bitCounter < dataLength.Length)
                {
                    g[counter] = dataLength[bitCounter];
                    counter++;
                    bitCounter++;
                }

                counter = 0;
                while (counter < 1 && bitCounter < dataLength.Length)
                {
                    b[counter] = dataLength[bitCounter];
                    counter++;
                    bitCounter++;
                }

                bmp.SetPixel(0, i, Color.FromArgb(
                    SteganographHelper.BitsToByte(r),
                    SteganographHelper.BitsToByte(g),
                    SteganographHelper.BitsToByte(b)
                    ));
            }
        }

        private static int ReadDataLength(Bitmap bmp)
        {
            BitArray dataLength = new BitArray(32);
            int bitCounter = 0;

            for (int i = 0; i < 12; i++)
            {
                Color color = bmp.GetPixel(0, i);
                BitArray r = SteganographHelper.ByteToBits(color.R);
                BitArray g = SteganographHelper.ByteToBits(color.G);
                BitArray b = SteganographHelper.ByteToBits(color.B);

                int counter = 0;
                while (counter < 1 && bitCounter < dataLength.Length)
                {
                    dataLength[bitCounter] = r[counter];
                    counter++;
                    bitCounter++;
                }

                counter = 0;
                while (counter < 1 && bitCounter < dataLength.Length)
                {
                    dataLength[bitCounter] = g[counter];
                    counter++;
                    bitCounter++;
                }

                counter = 0;
                while (counter < 1 && bitCounter < dataLength.Length)
                {
                    dataLength[bitCounter] = b[counter];
                    counter++;
                    bitCounter++;
                }
            }

            return SteganographHelper.BitsToInt32(dataLength);
        }
    }
}
