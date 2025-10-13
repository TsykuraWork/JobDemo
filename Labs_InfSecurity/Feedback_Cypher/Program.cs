using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
//12345678912345678912345678912345
namespace gost_magma
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;
            Console.Write("Введите имя входного файла: ");
            string input = Console.ReadLine();
            Console.Write("Введите имя выходного файла: ");
            string output = Console.ReadLine();
            //Console.Write("Введите ключ: ");
            //string key = Console.ReadLine();
            string key = "12345678912345678912345678912345";
            Console.Write("Выберите режим: 0 - зашифровать, 1 - расшифровать: ");
            int mode = Convert.ToInt32(Console.ReadLine());
            MagmaProvider magma = new MagmaProvider()
            {
                Key = key
            };
            if (mode == 0)
            {
                byte[] inBytes = File.ReadAllBytes(input);
                Result outBytes;
                outBytes = magma.CFBEncrypt(inBytes, 66467);
                using (var fs = new FileStream(output, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(BitConverter.GetBytes(outBytes.Length), 0, 4);
                    fs.Write(outBytes.Encrypted, 0, outBytes.Encrypted.Length);
                }
            }
            if (mode == 1)
            {
                byte[] inBytes = File.ReadAllBytes(input);
                Int32 length = BitConverter.ToInt32(inBytes, 0);
                Result inp = new Result(inBytes.Skip(4).ToArray(), length);
                byte[] outBytes;
                outBytes = magma.CFBDecrypt(inp, 66467);
                File.WriteAllBytes(output, outBytes);
            }

        }
    }
}
