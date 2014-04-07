using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chip8Sharp
{
    class Interpreter
    {
        private delegate void Executor();
        private readonly Dictionary<ushort, Executor> BaseOpcodeMap;
        private readonly CPU CPU;

        public Interpreter(CPU CPU)
        {
            this.CPU = CPU;
            this.BaseOpcodeMap = new Dictionary<ushort, Executor>();
        }

        public void DecodeExecute(ushort Instruction)
        {

        }
    }
}
