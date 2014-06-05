using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chip8Sharp;
using System.IO;
using System.Threading;
using System.Drawing;

namespace Chip8Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var Memory = new Memory(4096);
            using (var Stream = new FileStream("Connect4.ch8", FileMode.Open, FileAccess.Read))
            {
                Memory.Load(Stream, 0x0200);
            }
            
            var Output = new ConsoleOutput(Memory);
            var Input = new ConsoleInput();
            var CPU = new CPU(Memory, Input, Output);
            while (true)
            {
                if (Console.KeyAvailable)
                {

                }
                CPU.Cycle();
                Output.Tick();
                //Thread.Sleep(16);
            }
        }
    }
}
