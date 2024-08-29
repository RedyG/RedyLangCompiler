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
        public BrInstruction? BrInstruction { get; set; }

        public Block(BrInstruction? branchInstruction = null)
        {
            BrInstruction = branchInstruction;
        }

        public Block(List<Instruction> instructions, BrInstruction branchInstruction)
        {
            Instructions = instructions;
            BrInstruction = branchInstruction;
        }
    }
}
