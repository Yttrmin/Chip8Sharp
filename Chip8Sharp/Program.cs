using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Chip8Sharp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            byte[] Data;
            using(var Stream = new FileStream("test2.bin", FileMode.Open, FileAccess.Read))
            {
                Data = new byte[Stream.Length];
                Stream.Read(Data, 0, (int)Stream.Length);
            }
            var Output = new Output();
            var Input = new Input();
            var CPU = new CPU(Data, 0x200, Output, Input);
            while(true)
            {
                CPU.Cycle();
            }
        }
    }
}
