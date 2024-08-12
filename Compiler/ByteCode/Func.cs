using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.ByteCode
{
    public class Func
    {
        public Int16 ArgsCount { get; }
        public Int16 LocalsCount { get; }
        public List<Block> Blocks { get; } = new();
        public Block LastBlock => Blocks[^1];

        public void WriteTo(ByteList list)
        {
            Stack<int> branches = new();
            for (int i = 0; i < Blocks.Count; i++)
            {
                Block block = Blocks[i];
                int branchesCount = branches.Count;

                int initialSize = list.Count;
                Console.WriteLine("InitialSize: " + initialSize);

                foreach (var instruction in block.Instructions)
                    instruction.WriteTo(list);


                if (block.BranchInstruction.OpCode == BranchOpCode.Ret)
                {
                    list.Add(BranchInstruction.Ret);
                }
                else if (block.BranchInstruction.OpCode == BranchOpCode.Exit)
                {
                    list.Add(BranchInstruction.Exit);
                }
                else
                {
                    if (i + 1 == Blocks.Count)
                        throw new Exception("Last branch instruction should be a ret or exit");

                    Block nextBlock = Blocks[i + 1];
                    if (block.BranchInstruction.TrueBlock == nextBlock)
                    {
                        list.Add(BranchInstruction.BrFalse);
                    }
                    else if (block.BranchInstruction.FalseBlock == nextBlock)
                    {
                        list.Add(BranchInstruction.BrTrue);
                    }
                    else
                        throw new Exception("Invalid branch instruction for the moment");

                    branches.Push(list.Count);
                    list.Add((Int16)0);
                }

                if (branchesCount == 0)
                    continue;


                var size = (Int16)(list.Count - initialSize);
                Console.WriteLine(list[branches.Peek()]);
                Console.WriteLine("branch:" + branches.Peek());
                Console.WriteLine("Size: " + size);
                list.WriteAt(branches.Pop(), size);
            }
        }

        public Func(Int16 argsCount, Int16 localsCount)
        {
            ArgsCount = argsCount;
            LocalsCount = localsCount;
        }

        public Func(Int16 argsCount, Int16 localsCount, List<Block> blocks) : this(argsCount, localsCount)
        {
            Blocks = blocks;
        }
    }
}
