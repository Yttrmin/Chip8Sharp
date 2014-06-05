using Chip8Sharp;
using System;
using System.Collections.Generic;
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

            /*var Emulator = new Emulator();
            Emulator.Run(() =>
            {

            });*/
        }
    }
}
