using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chip8Sharp
{
    public class Emulator
    {
        private readonly CPU CPU;
        private readonly Memory Memory;
        private readonly IInput Input;
        private readonly IOutput Output;

        private const int MemorySizeInBytes = 4096;
        private const int ROMStartAddress = 0x0200;

        public Emulator(IInput Input, IOutput Output)
        {
            this.Memory = new Memory(MemorySizeInBytes);
            this.Input = Input;
            this.Output = Output;
        }

        public void LoadROM(Stream ROMStream)
        {
            this.Memory.Load(ROMStream, ROMStartAddress);
        }

        public void Pause()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Tick">Delegate the emulator will call to allow the host program to process and pass
        /// along input, render graphics, and play audio.</param>
        public void Run(Action Tick)
        {
            while(true)
            {

            }
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public void Step()
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        private class PipeManager
        {
            private readonly NamedPipeServerStream PipeServer;

            public PipeManager()
            {
                this.PipeServer = new NamedPipeServerStream("Chip8SharpPipe", PipeDirection.InOut, 1);
            }
        }
    }
}
