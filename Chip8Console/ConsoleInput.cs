using Chip8Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chip8Console
{
    class ConsoleInput : BaseInput
    {
        private readonly Dictionary<ConsoleKey, byte> KeyToKeypadMap;
        private byte KeyPressed;

        public ConsoleInput()
        {
            this.KeyToKeypadMap = new Dictionary<ConsoleKey, byte>()
            {
                {ConsoleKey.D1, 0},
                {ConsoleKey.D2, 1},
                {ConsoleKey.D3, 2},
                {ConsoleKey.D4, 3},
                {ConsoleKey.Q, 4},
                {ConsoleKey.W, 5},
                {ConsoleKey.E, 6},
                {ConsoleKey.R, 7},
                {ConsoleKey.A, 8},
                {ConsoleKey.S, 9},
                {ConsoleKey.D, 10},
                {ConsoleKey.F, 11},
                {ConsoleKey.Z, 12},
                {ConsoleKey.X, 13},
                {ConsoleKey.C, 14},
                {ConsoleKey.V, 15}
            };
        }

        public override byte WaitForNextKeyPress()
        {
            while(KeyPressed == byte.MaxValue)
            {
                this.Tick();
            }
            var Key = KeyPressed;
            KeyPressed = byte.MaxValue;
            return Key;
        }

        public override void Tick()
        {
            this.KeyPressed = byte.MaxValue;
            for(byte i = 0; i < this.KeyToKeypadMap.Count; i++)
            {
                this.KeyUp(i);
            }
            while(Console.KeyAvailable)
            {
                var Key = Console.ReadKey().Key;
                //@TODO TryGet
                if(this.KeyToKeypadMap.ContainsKey(Key))
                {
                    this.KeyUp(this.KeyToKeypadMap[Key]);
                    this.KeyPressed = this.KeyToKeypadMap[Key];
                }
            }
        }
    }
}
