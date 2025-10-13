using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO.Compression;
using System.IO;

namespace lab6
{
    public class AesEncrypter
    {
        AesCryptoServiceProvider aes = new AesCryptoServiceProvider()
        {
            Mode = CipherMode.CFB,
            KeySize = 128,
            Key = new byte[] { 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19, 0x20, 0x21, 0x22, 0x23, 0x24, 0x25 },
            IV = new byte[] { 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19, 0x20, 0x21, 0x22, 0x23, 0x24, 0x25 }
        };

        public byte[] Encrypt(byte[] data)
        {
            byte[] encrypted;

            byte[] hash = EncryptHelper.GetMD5Hash(data);
            byte[] hashLength = BitConverter.GetBytes(hash.Length);
            byte[] compressedData = EncryptHelper.GetCompressedBytes(data);
            byte[] compressedDataLength = BitConverter.GetBytes(compressedData.Length);

            ICryptoTransform cryptoTransform = aes.CreateEncryptor();

            using (MemoryStream ms = new MemoryStream())
            {
                ms.Write(hashLength, 0, 4);
                ms.Write(compressedDataLength, 0, 4);
                ms.Write(hash, 0, hash.Length);

                using (CryptoStream cs = new CryptoStream(ms, cryptoTransform, CryptoStreamMode.Write))
                {
                    cs.Write(compressedData, 0, compressedData.Length);
                }
                encrypted = ms.ToArray();
            }

            return encrypted;
        }

        public byte[] Decrypt(byte[] data)
        {
            try
            {
                byte[] decompressed;
                using (MemoryStream ms = new MemoryStream(data))
                {
                    byte[] hashLengthBytes = new byte[4];
                    ms.Seek(0, SeekOrigin.Begin);
                    ms.Read(hashLengthBytes, 0, 3);

                    byte[] decryptedDataLengthBytes = new byte[4];
                    ms.Seek(4, SeekOrigin.Begin);
                    ms.Read(decryptedDataLengthBytes, 0, 3);

                    int hashLengthInt = BitConverter.ToInt32(hashLengthBytes, 0);
                    int decryptedDataLengthInt = BitConverter.ToInt32(decryptedDataLengthBytes, 0);

                    byte[] hash = new byte[hashLengthInt];
                    ms.Seek(8, SeekOrigin.Begin);
                    ms.Read(hash, 0, hashLengthInt);

                    byte[] decryptedData = new byte[decryptedDataLengthInt];

                    ICryptoTransform cryptoTransform = aes.CreateDecryptor();
                    ms.Seek(8 + hashLengthInt, SeekOrigin.Begin);

                    using (CryptoStream cs = new CryptoStream(ms, cryptoTransform, CryptoStreamMode.Read))
                    {
                        cs.Read(decryptedData, 0, decryptedDataLengthInt);
                    }

                    decompressed = EncryptHelper.GetDecompressedBytes(decryptedData);
                }

                return decompressed;
            }
            catch (CryptographicException)
            {
                throw new CryptographicException("Закрытый ключ не совпадает с закрытым ключом," +
                    " которым файл был зашифрован.");
            }
        }
    }
}
