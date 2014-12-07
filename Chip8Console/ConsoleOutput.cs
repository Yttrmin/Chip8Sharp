using Chip8Sharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chip8Console
{
    class ConsoleOutput : BaseOutput
    {
        private readonly char[] Buffer;
        private readonly char FilledPixel = '█'; // X ■ █
        private readonly char EmptyPixel = ' ';

        public ConsoleOutput(Memory SystemMemory) : base(SystemMemory, 0x0F00, 0x0F00 + 256)
        {
            this.Buffer = new char[BaseOutput.NativeScreenWidth * BaseOutput.NativeScreenHeight];
            Console.CursorVisible = false;
            // Extending the height and buffer of the console by 1 (note NOT changing this.Buffer size)
            // fixes the flickering problem. Looks perfectly fine now.
            Console.SetWindowSize(NativeScreenWidth, NativeScreenHeight+1);
            Console.BufferWidth = NativeScreenWidth;
            Console.BufferHeight = NativeScreenHeight+1;
        }

        void GarbageBuffer()
        {
            var Random = new Random();
            for (var i = 0; i < Buffer.Length; i++)
            {
                Buffer[i] = Random.Next(2) == 1 ? FilledPixel : EmptyPixel;
            }
        }

        void UpdateBuffer()
        {
            var Index = 0;
            foreach (var Bit in this.VideoMemory.ReadBits(0, NativeScreenWidth * NativeScreenHeight / 8))
            {
                Buffer[Index] = Bit ? FilledPixel : EmptyPixel;
                Index++;
            }
        }

        void DrawFrame(Memory VideoMemory)
        {
            var Bitmap = new Bitmap(64, 32);
            for (var y = 0; y < 32; y++)
            {
                for (var x = 0; x < 64; x++)
                {
                    var Address = (y * 64 + x) / 8;
                    var Bit = this.VideoMemory.ReadBit(Address, x % 8);
                    if (Bit)
                    {
                        Bitmap.SetPixel(x, y, Color.Black);
                    }
                    else
                    {
                        Bitmap.SetPixel(x, y, Color.White);
                    }
                }
            }
            Bitmap.Save("frame.png", System.Drawing.Imaging.ImageFormat.Png);
        }

        void DrawBuffer()
        {
            Console.SetCursorPosition(0, 0);
            Console.Write(Buffer);
        }

        public override void Tick()
        {
            this.UpdateBuffer();
            this.DrawBuffer();
        }
    }
}
