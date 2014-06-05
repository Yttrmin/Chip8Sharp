using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chip8Sharp
{
    public interface IOutput : ITickable
    {
        Memory VideoMemory { get; }
        void ClearScreen();
    }

    public abstract class BaseOutput : IOutput
    {
        protected const int NativeScreenWidth = 64;
        protected const int NativeScreenHeight = 32;

        private readonly Memory _VideoMemory;
        public Memory VideoMemory { get { return this._VideoMemory; } }

        private const int GraphicsMemorySize = 64 * 32;

        public BaseOutput(Memory SystemMemory, int StartAddress, int EndAddress)
        {
            this._VideoMemory = new Memory(SystemMemory, StartAddress, EndAddress);
        }

        public void ClearScreen()
        {
            this._VideoMemory.Clear();
        }

        public abstract void Tick();
    }
}
