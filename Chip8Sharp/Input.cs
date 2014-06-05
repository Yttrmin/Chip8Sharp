using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chip8Sharp
{
    public interface IInput : ITickable
    {
        void KeyDown(byte Key);
        void KeyUp(byte Key);
        bool IsKeyDown(byte Key);
        bool IsKeyUp(byte Key);
        byte WaitForNextKeyPress();
    }

    public abstract class BaseInput : IInput
    {
        private readonly bool[] Keys;
        private const int KeyCount = 16;

        public BaseInput()
        {
            this.Keys = new bool[KeyCount];
        }

        public void KeyDown(byte Key)
        {
            //@TODO
            this.Keys[Key] = true;
        }

        public void KeyUp(byte Key)
        {
            //@TODO
            this.Keys[Key] = false;
        }

        public bool IsKeyDown(byte Key)
        {
            //@TODO
            return this.Keys[Key];
        }

        public bool IsKeyUp(byte Key)
        {
            //@TODO
            return !this.IsKeyDown(Key);
        }

        public abstract byte WaitForNextKeyPress();

        public abstract void Tick();
    }
}
