using Chip8Sharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chip8Console
{
    class EmulatorProgram
    {
        private static char[] Buffer;
        private const int ScreenWidth = 64;
        private const int ScreenHeight = 32;

        static void Main(string[] args)
        {
            Buffer = new char[ScreenWidth * ScreenHeight];
            Console.CursorVisible = false;
            Console.SetWindowSize(ScreenWidth, ScreenHeight);
            Console.BufferWidth = ScreenWidth;
            Console.BufferHeight = ScreenHeight;

            var Memory = new Memory(4096);
            var Output = new ConsoleOutput(Memory);
            var Input = new ConsoleInput();

            var Emulator = new Emulator(Memory, Input, Output);
            using (var Stream = new FileStream("Maze.c8", FileMode.Open, FileAccess.Read))
            {
                Emulator.LoadROM(Stream);
            }

            Emulator.Run(() =>
            {

            });
        }
    }
}
