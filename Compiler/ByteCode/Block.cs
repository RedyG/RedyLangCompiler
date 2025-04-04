using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.ByteCode
{
    /* TODO: optimizations passes
    
    - local set than directly get
    - not run pure code that gets popped anyway
    - simply branching, so instead of jumping in a block that has only a branch instruction, no other instruction, we can directly do that branch instruction.
    - simply branching2, just merge blocks that end with a br ( conditional ) for no reason with their target block.
    - not pop just before a ret_void or indirecly ( a jump to a ret_void or whatever )
    - function inlining
    - maybe transform all the alloca stuff when passing a value like i32 to another function to just pure return values and stuff
     
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
