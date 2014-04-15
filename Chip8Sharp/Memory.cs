using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chip8Sharp
{
    internal class Memory
    {
        private readonly byte[] MemoryArray;

        public Memory(int SizeInBytes)
        {
            this.MemoryArray = new byte[SizeInBytes];
        }

        public byte ReadByte(int Address)
        {
            return this.MemoryArray[Address];
        }

        public ushort ReadUInt16(int Address)
        {
            return BitConverter.ToUInt16(this.MemoryArray, Address);
        }

        public void WriteByte(int Address, byte Value)
        {
            this.MemoryArray[Address] = Value;
        }
    }
}
