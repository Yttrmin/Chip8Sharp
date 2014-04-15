using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chip8Sharp
{
    class CPU
    {
        private readonly Memory Memory;
        private readonly Stack<ushort> Stack;
        /// <summary>
        /// Registers, V0 through VF, each 8 bits in size.
        /// </summary>
        private readonly byte[] Registers;
        /// <summary>
        /// Index register, 16 bits in size.
        /// </summary>
        private byte IndexRegister;
        private ushort ProgramCounter;
        // Can be byte?
        private ushort StackPointer;
        //private ushort InsturctionPointer;
        private byte SoundTimer;
        private byte DelayTimer;
        private ulong Cycles;

        private const int RegisterCount = 16;
        private const int StackSize = 16;
        private const int MemorySize = 4096;

        public CPU()
        {
            this.Registers = new byte[RegisterCount];
            this.Stack = new Stack<ushort>(StackSize);
            this.Memory = new Memory(MemorySize);
        }

        public CPU(byte[] Program, int StartAddress) : this()
        {
            this.LoadMemory(Program, StartAddress);
            this.ProgramCounter = (ushort)StartAddress;
        }

        private void ConstructOpcodeMaps()
        {

        }

        public void Cycle()
        {
            var InstructionPointer = this.Fetch();
            this.ProgramCounter += 2;

            if(this.SoundTimer > 0)
            {
                this.SoundTimer--;
            }
            if(this.DelayTimer > 0)
            {
                this.DelayTimer--;
            }
            this.Cycles++;
        }

        public void LoadMemory(byte[] Contents, int StartAddress)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Fetches the opcode at Memory[PC] and increments the PC by 2.
        /// </summary>
        /// <returns>Opcode.</returns>
        private ushort Fetch()
        {
            var Result = this.Memory.ReadUInt16(this.ProgramCounter);
            return Result;
        }

        private void DecodeExecute(ushort Instruction)
        {

        }
    }
}
