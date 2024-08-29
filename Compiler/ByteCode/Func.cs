using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.ByteCode
{
    public class Func
    {
        public int ParamsCount { get; }
        public int LocalsCount { get; set; }
        public List<Block> Blocks { get; } = new();
        public Block LastBlock => Blocks[^1];

        public Block AddBlock()
        {
            var block = new Block();
            Blocks.Add(block);
            return block;
        }

        public void WriteTo(ByteList list)
        {
            Stack<int> branches = new();
            for (int i = 0; i < Blocks.Count; i++)
            {
                Block block = Blocks[i];

                if (block.BrInstruction == null)
                    throw new Exception("Branch instruction is null");

                int branchesCount = branches.Count;

                int initialSize = list.Count;
                Console.WriteLine("InitialSize: " + initialSize);

                foreach (var instruction in block.Instructions)
                    instruction.WriteTo(list);

                if (block.BrInstruction.Value.OpCode == BrOpCode.Ret)
                {
                    list.Add(BrInstruction.Ret);
                }
                else if (block.BrInstruction.Value.OpCode == BrOpCode.Exit)
                {
                    list.Add(BrInstruction.Exit);
                }
                else
                {
                    if (i + 1 == Blocks.Count)
                        throw new Exception("Last branch instruction should be a ret or exit");

                    Block nextBlock = Blocks[i + 1];
                    if (block.BrInstruction.Value.TrueBlock == nextBlock)
                    {
                        list.Add(BrInstruction.BrFalse);
                    }
                    else if (block.BrInstruction.Value.FalseBlock == nextBlock)
                    {
                        list.Add(BrInstruction.BrTrue);
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

        public Func(int paramsCount, int localsCount)
        {
            ParamsCount = paramsCount;
            LocalsCount = localsCount;
        }

        public Func(int paramsCount, int localsCount, List<Block> blocks) : this(paramsCount, localsCount)
        {
            Blocks = blocks;
        }
    }
}
