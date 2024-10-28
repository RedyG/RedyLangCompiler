using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.ByteCode
{
    public class Func
    {
        public bool Main { get; set; }
        public Module? Module { get; set; } = null;
        public int ParamsCount { get; set; }
        public int LocalsCount { get; set; }
        public List<Block> Blocks { get; } = new();
        public Block LastBlock => Blocks[^1];

        public Block AddBlock()
        {
            var block = new Block();
            Blocks.Add(block);
            return block;
        }

        public int GetId() => Module!.Funcs.IndexOf(this) + Module.ImportedFuncs.Count;

        public void WriteTo(Module module, ByteList list)
        {
            List<KeyValuePair<Block, (int Pos, Int16 Size)>> branches = new();

            for (int i = 0; i < Blocks.Count; i++)
            {
                Block block = Blocks[i];

                if (block.BrInstruction == null)
                    throw new Exception("Branch instruction is null");

                int branchesCount = branches.Count;

                int initialSize = list.Count;
                Console.WriteLine("InitialSize: " + initialSize);

                foreach (var instruction in block.Instructions)
                    instruction.WriteTo(module, list);

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

                    Block? trueBlock = block.BrInstruction.Value.TrueBlock;
                    Block? falseBlock = block.BrInstruction.Value.FalseBlock;
                    Block nextBlock = Blocks[i + 1];
                    if (trueBlock == nextBlock && falseBlock != null)
                    {
                        list.Add(BrInstruction.BrFalse);
                        branches.Add(new(falseBlock, (list.Count, 0)));
                    }
                    else if (falseBlock == nextBlock && trueBlock != null)
                    {
                        list.Add(BrInstruction.BrTrue);
                        branches.Add(new(trueBlock, (list.Count, 0)));
                    }
                    else if (trueBlock != null && falseBlock == null)
                    {
                        list.Add(BrInstruction.Br);
                        branches.Add(new(trueBlock, (list.Count, 0)));
                    }
                    else
                        throw new Exception("Invalid branch instruction");

                    list.Add((Int16)0);
                }

                if (branchesCount == 0)
                    continue;

                var size = (Int16)(list.Count - initialSize);
                for (int j = 0; j < branchesCount; j++)
                {
                    var branch = branches[j];
                    if (branch.Key == block)
                    {
                        list.WriteAt(branch.Value.Pos, branch.Value.Size);
                        continue;
                    }
                    branches[j] = new(branch.Key, (branch.Value.Pos, (Int16)(branch.Value.Size + size)));
                }
                branches.RemoveAll(branch => branch.Key == block);
            }
        }

        public Func(int paramsCount, int localsCount, bool main = false)
        {
            ParamsCount = paramsCount;
            LocalsCount = localsCount;
            Main = main;
        }

        public Func(int paramsCount, int localsCount, List<Block> blocks, bool main = false) : this(paramsCount, localsCount, main)
        {
            Blocks = blocks;
        }
    }
}
