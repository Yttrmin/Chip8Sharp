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
        private const int ScreenWidth = 64;
        private const int ScreenHeight = 32;
        private static char[] Buffer;

        static void Main(string[] args)
        {
            Buffer = new char[ScreenWidth*ScreenHeight];
            Console.CursorVisible = false;
            Console.SetWindowSize(ScreenWidth, ScreenHeight);
            Console.BufferWidth = ScreenWidth;
            Console.BufferHeight = ScreenHeight;

            byte[] Data;
            using (var Stream = new FileStream("Maze.c8", FileMode.Open, FileAccess.Read))
            {
                Data = new byte[Stream.Length];
                Stream.Read(Data, 0, (int)Stream.Length);
            }
            var Output = new Output();
            var Input = new Input();
            var CPU = new CPU(Data, 0x200, Output, Input);
            //CPU.VideoMemory.SetAll((x, y) => { if (x % 2 != 0) return true; return false; });
            //CPU.VideoMemory.SetAll((x, y) => {return true; });
            while (true)
            {
                if (Console.KeyAvailable)
                {

                }
                CPU.Cycle();
                UpdateBuffer(CPU.VideoMemory);
                //GarbageBuffer();
                DrawBuffer();
                Thread.Sleep(16);
            }
        }

        static void GarbageBuffer()
        {
            var Random = new Random();
            for(var i = 0; i < Buffer.Length; i++)
            {
                Buffer[i] = Random.Next(2) == 1 ? 'X' : ' ';
            }
        }

        static void UpdateBuffer(Memory VideoMemory)
        {
            var Index = 0;
            foreach (var Bit in VideoMemory.ReadBits(0, ScreenWidth * ScreenHeight / 8))
            {
                Buffer[Index] = Bit ? 'X' : ' ';
                Index++;
            }
        }

        static void DrawFrame(Memory VideoMemory)
        {
            var Bitmap = new Bitmap(64, 32);
            for(var y = 0; y < 32; y++)
            {
                for (var x = 0; x < 64; x++)
                {
                    var Address = (y * 64 + x) / 8;
                    var Bit = VideoMemory.ReadBit(Address, x % 8);
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

        static void DrawBuffer()
        {
            //Console.Clear();
            Console.SetCursorPosition(0, 0);
            Console.Write(Buffer);
        }
    }
}
