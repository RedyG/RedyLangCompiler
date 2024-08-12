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
        public BranchInstruction BranchInstruction { get; }

        public Block(BranchInstruction branchInstruction)
        {
            BranchInstruction = branchInstruction;
        }

        public Block(List<Instruction> instructions, BranchInstruction branchInstruction)
        {
            Instructions = instructions;
            BranchInstruction = branchInstruction;
        }
    }
}
