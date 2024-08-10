using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.ByteCode
{
    public class Block
    {
        public List<Instruction> Instructions { get; } = new();

    }
}
