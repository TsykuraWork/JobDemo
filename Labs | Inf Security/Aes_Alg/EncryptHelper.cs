using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace lab6
{
    public static class EncryptHelper
    {
        public static byte[] GetMD5Hash(byte[] data)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            return md5.ComputeHash(data);
        }

        public static byte[] GetCompressedBytes(byte[] data)
        {
            byte[] compressed;

            using (MemoryStream output = new MemoryStream())
            {
                using (DeflateStream ds = new DeflateStream(output, CompressionMode.Compress))
                {
                    ds.Write(data, 0, data.Length);
                }
                compressed = output.ToArray();
            }
            return compressed;
        }

        public static byte[] GetDecompressedBytes(byte[] data)
        {
            MemoryStream input = new MemoryStream(data);
            MemoryStream output = new MemoryStream();
            using (DeflateStream dstream = new DeflateStream(input, CompressionMode.Decompress))
            {
                dstream.CopyTo(output);
            }
            return output.ToArray();
        }

        public static byte[] GetHashFromEncryptedFileData(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data))
            {
                int hashLen = BitConverter.ToInt32(data, 0);
                byte[] hash = new byte[hashLen];
                ms.Seek(8, SeekOrigin.Begin);
                ms.Read(hash, 0, hashLen);
                return hash;
            }
        }

        public static bool HashEquals(byte[] hash1, byte[] hash2)
        {
            if (hash1.Length != hash2.Length)
                return false;

            for (int i = 0; i < hash1.Length; i++)
            {
                if (hash1[i] != hash2[i])
                    return false;
            }

            return true;
        }
    }
}
