using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.ByteCode
{
    /* TODO: optimizations passes
    
    - local set than directly get
    - poping pure values instead of just not running that code since it has no side effect
    - simply branching, so instead of jumping in a block that has only a branch instruction, no other instruction, we can directly do that branch instruction.
    - simply branching2, just merge blocks that end with a br ( conditional ) for no reason with their target block.
     
    */
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
