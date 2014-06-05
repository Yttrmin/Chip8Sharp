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
            using (var Stream = new FileStream("Maze.c8", FileMode.Open, FileAccess.Read))
            {
                Memory.Load(Stream, 0x0200);
            }
            
            var Output = new ConsoleOutput(Memory);
            var Input = new Input();
            var CPU = new CPU(Memory, Output);
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
