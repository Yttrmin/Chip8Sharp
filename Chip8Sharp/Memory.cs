using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chip8Sharp
{
    public sealed class Memory
    {
        private readonly IList<byte> MemoryArray;

        public Memory(int SizeInBytes)
        {
            this.MemoryArray = new byte[SizeInBytes];
        }

        public Memory(Memory Other, int StartAddress, int EndAddress)
        {
            // Boxed, but ArraySegment is immutable so no harm done.
            this.MemoryArray = new ArraySegment<byte>((byte[])Other.MemoryArray, StartAddress, EndAddress - StartAddress);
        }

        public byte this[int Address]
        {
            get {return this.ReadByte(Address);}
            set { this.WriteByte(Address, value); }
        }

        public void Load(Stream Stream, int StartAddress)
        {
            for (var i = 0; i < Stream.Length; i++)
            {
                this.WriteByte(StartAddress+i, (byte)Stream.ReadByte());
            }
        }

        public IEnumerable<bool> ReadBits(int StartAddress, int ByteAmount)
        {
            for(var i = StartAddress; i < StartAddress+ByteAmount; i++)
            {
                var Byte = this.ReadByte(i);
                for(var u = 7; u >= 0; u--)
                {
                    yield return ((Byte >> u) & 1) == 1;
                }
            }
        }

        public bool ReadBit(int Address, int Bit)
        {
            return this.MemoryArray[Address] >> (7 - Bit) == 1;
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

        public void WriteBit(int Bit, bool Value)
        {
            var Address = Bit / 8;
            var BitOffset = Bit % 8;

            this.WriteBit(Address, (byte)BitOffset, Value);
        }

        public void WriteBit(int Address, byte Bit, bool Value)
        {
            if(Bit > 8)
            {
                throw new ArgumentException("Bit exceeds size of byte.");
            }
            var Byte = this.ReadByte(Address);
            var OldByte = Byte;
            if(Value)
            {
                Byte |= (byte)(0x80 >> Bit);
                Debug.Assert(Byte >= OldByte);
            }
            else
            {
                unchecked
                {
                    Byte &= (byte)~(0x80 >> Bit);
                    Debug.Assert(Byte <= OldByte);
                }
            }
            this.WriteByte(Address, Byte);
        }

        public void Clear()
        {
            var ArrayMemory = this.MemoryArray as byte[] ?? ((ArraySegment<byte>)this.MemoryArray).Array;
            Array.Clear(ArrayMemory, 0, this.MemoryArray.Count);
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
