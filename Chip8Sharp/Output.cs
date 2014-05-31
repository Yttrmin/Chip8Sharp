using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chip8Sharp
{
    internal class Output
    {
        private readonly Memory GraphicsMemory;

        private const int GraphicsMemorySize = 64 * 32;

        public Output()
        {
            this.GraphicsMemory = new Memory(GraphicsMemorySize);
        }

        public void ClearMemory()
        {
            this.GraphicsMemory.Clear();
        }
    }
}
