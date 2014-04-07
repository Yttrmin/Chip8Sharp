using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chip8Sharp
{
    class CPU
    {
        

        private readonly bool[] GraphicsMemory;
        private readonly byte[] Memory;
        private readonly ushort[] Stack;
        /// <summary>
        /// Registers, V0 through VF, each 8 bits in size.
        /// </summary>
        private readonly byte[] V;
        /// <summary>
        /// Index register, 16 bits in size.
        /// </summary>
        private byte I;
        private ushort PC;
        private ushort SP;
        private ushort IP;

        private const int RegisterCount = 16;
        private const int MemorySize = 4096;
        private const int GraphicsMemorySize = 64 * 32;
        private const int StackSize = 16;

        public CPU()
        {
            this.V = new byte[RegisterCount];
            this.Memory = new byte[MemorySize];
            this.GraphicsMemory = new bool[GraphicsMemorySize];
            
        }

        private void ConstructOpcodeMaps()
        {

        }

        public void LoadMemory(byte[] Contents, int StartAddress)
        {
            Array.Copy(Contents, 0, this.Memory, StartAddress, Contents.Length);
        }

        /// <summary>
        /// Fetches the opcode at Memory[PC] and increments the PC by 2.
        /// </summary>
        /// <returns>Opcode.</returns>
        private ushort Fetch()
        {
            var Result = BitConverter.ToUInt16(this.Memory, this.PC);
            PC += 2;
            return Result;
        }

        private void DecodeExecute(ushort Instruction)
        {

        }
    }
}
