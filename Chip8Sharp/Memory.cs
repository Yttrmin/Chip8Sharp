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
            // Don't use BitConverter.ToUInt16 since this is (most likely) running on a little-endian
            // system. CHIP8 is big-endian.
            return (ushort)(this.MemoryArray[Address] << 8 | this.MemoryArray[Address + 1]);
        }

        public void WriteByte(int Address, byte Value)
        {
            this.MemoryArray[Address] = Value;
        }

        public void Clear()
        {
            Array.Clear(this.MemoryArray, 0, this.MemoryArray.Length);
        }

        private void MemoryEvent(int StartAddress, int Size, MemoryAccess AccessType)
        {
            throw new NotImplementedException();
        }
    }

    enum MemoryAccess
    {
        None = 0,
        Read = 1,
        Write = 2,
        Execute = 4
    }
}
