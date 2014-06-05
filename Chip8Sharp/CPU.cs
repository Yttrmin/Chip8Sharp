using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chip8Sharp
{
    public class CPU
    {
        private readonly Input Input;
        private readonly IOutput Output;
        public Memory Memory { get; private set; }
        public Memory VideoMemory { get; private set; }
        private readonly Stack<ushort> Stack;
        private readonly Random Random;
        /// <summary>
        /// Registers, V0 through VF, each 8 bits in size.
        /// </summary>
        private readonly byte[] Registers;
        /// <summary>
        /// Index register, 16 bits in size.
        /// </summary>
        private ushort IndexRegister;
        private ushort ProgramCounter;
        // Can be byte?
        //private ushort StackPointer;
        private byte FlagRegister { get { return this.Registers[0xF]; } set { this.Registers[0xF] = value; } }
        //private ushort InsturctionPointer;
        private byte SoundTimer;
        private byte DelayTimer;
        private ulong Cycles;

        private const int RegisterCount = 16;
        private const int StackSize = 16;
        private const int MemorySize = 4096;
        private const int ScreenWidth = 64;
        private const int ScreenHeight = 32;
        public const int VideoMemoryStart = 0x0F00;
        public const int VideoMemoryEnd = VideoMemoryStart + 256;
        private const int ProgramROMStart = 0x0200;
        private CPU()
        {
            this.Registers = new byte[RegisterCount];
            this.Stack = new Stack<ushort>(StackSize);
            //this.Memory = new Memory(MemorySize);
            //this.VideoMemory = new Memory(this.Memory, VideoMemoryStart, VideoMemoryEnd);
            this.Random = new Random();
        }

        public CPU(Memory SystemMemory, IOutput Output) : this()
        {
            this.Memory = SystemMemory;
            this.Output = Output;
            this.VideoMemory = this.Output.VideoMemory;
            this.ProgramCounter = ProgramROMStart;
        }

        /// <summary>
        /// Resets CPU state to default. Does not affect other components (Input, Memory, Output, etc).
        /// </summary>
        /// <param name="ProgramCounter">Address of next instruction to execute after reset.</param>
        public void Reset(ushort ProgramCounter)
        {
            this.ProgramCounter = ProgramCounter;
            this.SoundTimer = 0;
            this.DelayTimer = 0;
            this.IndexRegister = 0;
            this.Stack.Clear();
            for (var i = 0; i < this.Registers.Length; i++ )
            {
                this.Registers[i] = 0;
            }
            this.Cycles = 0;
        }

        private void ConstructOpcodeMaps()
        {

        }

        public void Cycle()
        {
            var InstructionPointer = this.Fetch();
            this.ProgramCounter += 2;

            this.DecodeExecute(InstructionPointer);

            if(this.DelayTimer > 0)
            {
                this.DelayTimer--;
            }
            if (this.SoundTimer > 0)
            {
                this.SoundTimer--;
            }
            this.Cycles++;
        }

        public void Cycle(int OpCode)
        {
            throw new NotImplementedException();
        }

        public string DumpAdjacentMemory(byte Lines)
        {
            var Builder = new StringBuilder();
            for(ushort Address = (ushort)(Math.Max(0, this.ProgramCounter - Lines/2)); 
                Address < this.ProgramCounter + Lines/2; Address+=2)
            {
                Builder.AppendFormat("0x{0:X4}: {1:X4} {2}{3}", Address, this.Memory.ReadUInt16(Address),
                    Address == this.ProgramCounter ? "<------------------- Program Counter" : string.Empty, 
                    Environment.NewLine);
            }
            return Builder.ToString();
        }

        [Obsolete("Use Memory")]
        public void LoadMemory(byte[] Contents, int StartAddress)
        {
            var Address = StartAddress;
            for(var i = 0; i < Contents.Length; i++)
            {
                this.Memory.WriteByte(Address, Contents[i]);
                Address++;
            }
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

        private struct SpritePixel
        {
            public readonly bool Value;
            public readonly byte X, Y;

            public SpritePixel(byte X, byte Y, bool Value)
            {
                this.Value = Value;
                this.X = X;
                this.Y = Y;
            }
        }

        private IEnumerable<SpritePixel> GetSprite(int StartAddress, int StartX, int StartY, byte Height)
        {
            var X = (byte)StartX;
            var Y = (byte)StartY;
            for(var Address = StartAddress; Address < StartAddress+Height; Address++)
            {
                // Went off screen vertically, no hope of getting back.
                if(Y >= ScreenHeight)
                {
                    yield break;
                }
                var Byte = this.Memory.ReadByte(Address);
                for (var i = 0; i < 8; i++)
                {
                    yield return new SpritePixel(X, Y, (Byte >> (7-i)) % 2 != 0);
                    X++;
                    // Went off screen horizontally, break to try the next row.
                    if(X >= ScreenWidth)
                    {
                        break;
                    }
                }
                Y++;
                X = (byte)StartX;
            }
        }

        private void DecodeExecute(ushort Instruction)
        {
            byte Value;
            byte RegisterIndex, RegisterIndexY;
            int TempValue;
            ushort Address;

            var FirstNibble = (byte)((Instruction & 0xF000) >> 12);
            switch(FirstNibble)
            {
                case 0x0:
                    switch(Instruction & 0x00FF)
                    {
                        case 0x00E0:
                            // CLS. Clear Screen. No arguments.
                            this.Output.ClearScreen();
                            break;
                        case 0x00EE:
                            // RET. Return from subroutine. No arguments.
                            this.ProgramCounter = this.Stack.Pop();
                            break;
                    }
                    goto default;
                case 0x1:
                    // JMP. Jump to address 0nnn. 1 argument 0x1nnn.
                    this.ProgramCounter = (ushort)(Instruction & 0x0FFF);
                    break;
                case 0x2:
                    // CALL. Call subroutine at address 0nnn. 1 argument 0x2nnn.
                    this.Stack.Push(this.ProgramCounter);
                    this.ProgramCounter = (ushort)(Instruction & 0x0FFF);
                    break;
                case 0x3:
                    // SE. Skip next instruction if the value in register x is equal to value kk. 2 arguments. 0x3xkk.
                    RegisterIndex = (byte)((Instruction & 0x0F00) >> 8);
                    Value = (byte)(Instruction & 0x00FF);
                    if(this.Registers[RegisterIndex] == Value)
                    {
                        this.ProgramCounter += 2;
                    }
                    break;
                case 0x4:
                    // SNE. Skip next instruction if the value in register x is NOT equal to value kk. 2 arguments. 0x4xkk.
                    RegisterIndex = (byte)((Instruction & 0x0F00) >> 8);
                    Value = (byte)(Instruction & 0x00FF);
                    if(this.Registers[RegisterIndex] != Value)
                    {
                        this.ProgramCounter += 2;
                    }
                    break;
                case 0x5:
                    // SE. Skip next instruction if the value in register x 
                    // is equal to the value in register y. 2 arguments. 0x5xy0.
                    RegisterIndex = (byte)((Instruction & 0x0F00) >> 8);
                    RegisterIndexY = (byte)(Instruction & 0x00F0);
                    if(this.Registers[RegisterIndex] == this.Registers[RegisterIndexY])
                    {
                        this.ProgramCounter += 2;
                    }
                    break;
                case 0x6:
                    // LD. Load value kk into register x. 2 arguments. 0x6xkk.
                    RegisterIndex = (byte)((Instruction & 0x0F00) >> 8);
                    Value = (byte)(Instruction & 0x00FF);
                    this.Registers[RegisterIndex] = Value;
                    break;
                case 0x7:
                    // ADD. Adds the value kk to register x, storing the result in register x. 2 arguments. 0x7xkk.
                    RegisterIndex = (byte)((Instruction & 0x0F00) >> 8);
                    Value = (byte)(Instruction & 0x00FF);
                    unchecked
                    {
                        this.Registers[RegisterIndex] += Value;
                    }
                    break;
                case 0x8:
                    RegisterIndex = (byte)((Instruction & 0x0F00) >> 8);
                    RegisterIndexY = (byte)(Instruction & 0x00F0);
                    switch((byte)(Instruction & 0x000F))
                    {
                        case 0x0:
                            // LD Vx, Vy. Puts value of Vy into Vx. 2 arguments. 0x8xy0.
                            this.Registers[RegisterIndex] = this.Registers[RegisterIndexY];
                            break;
                        case 0x1:
                            // OR Vx, Vy. Puts the result of Vx OR Vy into Vx. 2 arguments. 0x8xy1.
                            this.Registers[RegisterIndex] = (byte)(this.Registers[RegisterIndex] | this.Registers[RegisterIndexY]);
                            break;
                        case 0x2:
                            // AND Vx, Vy. Puts the result of Vx AND Vy into Vx. 2 arguments. 0x8xy2.
                            this.Registers[RegisterIndex] = (byte)(this.Registers[RegisterIndex] & this.Registers[RegisterIndexY]);
                            break;
                        case 0x3:
                            // XOR Vx, Vy. Puts the result of Vx XOR Vy into Vx. 2 arguments. 0x8xy3.
                            this.Registers[RegisterIndex] = (byte)(this.Registers[RegisterIndex] ^ this.Registers[RegisterIndexY]);
                            break;
                        case 0x4:
                            // ADD Vx, Vy. Puts value of Vy + Vx into Vx. If result overflows, set VF to 1, otherwise 0. 2 arguments. 0x8xy4.
                            TempValue = this.Registers[RegisterIndex] + this.Registers[RegisterIndexY];
                            if(TempValue > byte.MaxValue)
                            {
                                this.FlagRegister = 1;
                            }
                            else
                            {
                                this.FlagRegister = 0;
                            }
                            this.Registers[RegisterIndex] = (byte)unchecked(this.Registers[RegisterIndex] + this.Registers[RegisterIndexY]);
                            break;
                        case 0x5:
                            // SUB Vx, Vy. If Vx > Vy, set VF to 1, otherwise 0. Puts result of Vx - Vy into Vx. 2 arguments. 0x8xy5.
                            if (this.Registers[RegisterIndex] > this.Registers[RegisterIndexY])
                            {
                                this.FlagRegister = 1;
                            }
                            else
                            {
                                this.FlagRegister = 0;
                            }
                            this.Registers[RegisterIndex] = (byte)unchecked(this.Registers[RegisterIndex] - this.Registers[RegisterIndexY]);
                            break;
                        case 0x6:
                            // SHR Vx. VF = least-sig bit of Vx. Then shift Vx right once and store the result in Vx. 1 argument. 0x8xy6.
                            this.FlagRegister = (byte)(this.Registers[RegisterIndex] & 0x01);
                            this.Registers[RegisterIndex] >>= 1;
                            break;
                        case 0x7:
                            // SUBN Vx, Vy. If Vy > Vx, VF = 1, else 0. Then set Vx to Vy - Vx. 2 arguments. 0x8xy7.
                            if (this.Registers[RegisterIndexY] > this.Registers[RegisterIndexY])
                            {
                                this.FlagRegister = 1;
                            }
                            else
                            {
                                this.FlagRegister = 0;
                            }
                            this.Registers[RegisterIndex] = (byte)(this.Registers[RegisterIndexY] - this.Registers[RegisterIndex]);
                            break;
                        case 0xE:
                            // SGL Vx. VF = most-sig bit of Vx. Then shift Vx left once and store the result in Vx. 1 argument. 0x8xy7.
                            this.FlagRegister = (byte)(this.Registers[RegisterIndex] & 0x80);
                            this.Registers[RegisterIndex] <<= 1;
                            break;
                    }
                    goto default;
                case 0xA:
                    // LD I. Set value of index register to the address nnn. 1 argument. 0xAnnn.
                    this.IndexRegister = (ushort)(Instruction & 0x0FFF);
                    break;
                case 0xB:
                    // JP V0, addr. Jump to memory location (V0 + addr). 1 argument. 0xBnnn.
                    this.ProgramCounter = (ushort)(this.Registers[0] + (Instruction & 0x0FFF));
                    break;
                case 0xC:
                    // RND Vx, byte. Set value of register Vx to a random byte AND'd with value kk. 2 arguments. 0xCxkk.
                    var RandByte = (byte)this.Random.Next(byte.MaxValue + 1);
                    RegisterIndex = (byte)((Instruction & 0x0F00) >> 8);
                    Value = (byte)(Instruction & 0x00FF);
                    this.Registers[RegisterIndex] = (byte)(Value & RandByte);
                    break;
                case 0xD:
                    // DRW Vx, Vy, nibble. 
                    RegisterIndex = (byte)((Instruction & 0x0F00) >> 8);
                    var ScreenX = this.Registers[RegisterIndex];
                    RegisterIndex = (byte)((Instruction & 0x00F0) >> 4);
                    var ScreenY = this.Registers[RegisterIndex];
                    var Height = (byte)(Instruction & 0x000F);
                    this.FlagRegister = 0;
                    foreach(var BitData in this.GetSprite(this.IndexRegister, ScreenX, ScreenY, Height))
                    {
                        Address = (ushort)((BitData.Y * ScreenWidth + BitData.X)/8);
                        var OldValue = this.VideoMemory.ReadByte(Address);
                        this.VideoMemory.WriteBit(Address, (byte)(BitData.X % 8), BitData.Value);
                        if(OldValue > this.VideoMemory.ReadByte(Address))
                        {
                            this.FlagRegister = 1;
                        }
                    }
                    break;
                case 0xE:
                    switch((byte)(Instruction & 0x00FF))
                    {
                        case 0x9E:
                            // SKP Vx. Skip the next instruction if the x key is down. 1 argument. 0xEx9E.
                            Value = (byte)(Instruction & 0x0F00);
                            if(this.Input.IsKeyDown(Value))
                            {
                                this.ProgramCounter += 2;
                            }
                            break;
                        case 0xA1:
                            // SKNP Vx. Skip the next instruction if the x key is up. 1 argument. 0xExA1.
                            Value = (byte)(Instruction & 0x0F00);
                            if(this.Input.IsKeyUp(Value))
                            {
                                this.ProgramCounter += 2;
                            }
                            break;
                    }
                    goto default;
                case 0xF:
                    switch ((byte)(Instruction & 0x00FF))
                    {
                        case 0x07:
                            // LD Vx, DT. Set value of Vx to delay timer's value. 1 argument. 0xFx07.
                            RegisterIndex = (byte)((Instruction & 0x0F00) >> 8);
                            this.Registers[RegisterIndex] = this.DelayTimer;
                            break;
                        case 0x0A:
                            // LD Vx, K. Waits for a key press, stores value of key in Vx. 1 argument. 0xFx0A.
                            RegisterIndex = (byte)((Instruction & 0x0F00) >> 8);
                            this.Registers[RegisterIndex] = this.Input.WaitForNextKeyPress();
                            break;
                        case 0x15:
                            // LD DT, Vx. Set value of delay timer to Vx. 1 argument. 0xFx15.
                            RegisterIndex = (byte)((Instruction & 0x0F00) >> 8);
                            this.DelayTimer = this.Registers[RegisterIndex];
                            break;
                        case 0x18:
                            // LD ST, Vx. Set value of sound timer to Vx. 1 argument. 0xFx18:
                            RegisterIndex = (byte)((Instruction & 0x0F00) >> 8);
                            this.SoundTimer = this.Registers[RegisterIndex];
                            break;
                        case 0x1E:
                            // ADD I, Vx. Set value of I to I + Vx. 1 argument. 0xFx1E.
                            RegisterIndex = (byte)((Instruction & 0x0F00) >> 8);
                            this.IndexRegister = (ushort)(this.IndexRegister + this.Registers[RegisterIndex]);
                            break;
                        case 0x29:
                            // LD F, Vx. Set value of I to location for hex sprite cooresponding to Vx. 1 argument. 0xFx29.
                            throw new NotImplementedException();
                        case 0x33:
                            // LD B, Vx. Determine base-10 value of Vx. Hundreds digit goes to I, tens to I+1, and ones to I+2. 1 argument. 0xFx33.
                            throw new NotImplementedException();
                        case 0x55:
                            // LD [I], Vx. Store registers V0 through Vx in memory starting at address I. 1 argument. 0xFx55.
                            Value = (byte)(Instruction & 0x0F00);
                            Address = this.IndexRegister;
                            //@BUG Is the end inclusive or exclusive?
                            for (var i = 0; i < Value; i++ )
                            {
                                this.Memory.WriteByte(Address, this.Registers[i]);
                                Address++;
                            }
                            break;
                        case 0x65:
                            // LD Vx, [I]. Set values of registers V0 through Vx from memory starting at address I. 1 argument. 0xFx65.
                            Value = (byte)(Instruction & 0x0F00);
                            Address = this.IndexRegister;
                            //@BUG Is the end inclusive or exclusive?
                            for (var i = 0; i < Value; i++ )
                            {
                                this.Registers[i] = this.Memory.ReadByte(Address);
                                Address++;
                            }
                            break;
                    }
                    goto default;
                default:
                    //@TODO - More info.
                    throw new ArgumentException(String.Format("Tried to execute invalid opcode {0:X4}", Instruction));
            }
        }
    }
}