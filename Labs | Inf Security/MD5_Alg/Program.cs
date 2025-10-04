using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp4
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Vvedite stroky: ");
            string input = Console.ReadLine();
            MD5 objectName = new MD5(input);
            Console.Write(objectName.GenHash());
            Console.ReadLine();
        }
    }
}
