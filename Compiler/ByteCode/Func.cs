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
            List<(int Pos, Block Block)> branches = new();
            Dictionary<Block, int> blockPositions = new();

            for (int i = 0; i < Blocks.Count; i++)
            {
                Block block = Blocks[i];
                blockPositions.Add(block, list.Count);

                foreach (var instruction in block.Instructions)
                    instruction.WriteTo(module, list);

                if (block.BrInstruction == null)
                {
                    if (i + 1 == Blocks.Count)
                        throw new Exception("Last block should have a branch instruction");
                    else
                        continue;
                }

                if (block.BrInstruction.Value.OpCode == BrOpCode.Ret)
                {
                    list.Add(BrInstruction.Ret);
                }
                else if (block.BrInstruction.Value.OpCode == BrOpCode.RetVoid)
                {
                    list.Add(BrInstruction.RetVoid);
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
                        branches.Add((list.Count, falseBlock));
                    }
                    else if (falseBlock == nextBlock && trueBlock != null)
                    {
                        list.Add(BrInstruction.BrTrue);
                        branches.Add((list.Count, trueBlock));
                    }
                    else if (trueBlock != null && falseBlock == null)
                    {
                        list.Add(BrInstruction.Br);
                        branches.Add((list.Count, trueBlock));
                    }
                    else
                        throw new Exception("Invalid branch instruction");

                    list.Add((Int16)0);
                }
            }

            foreach (var branch in branches)
            {
                if (blockPositions.TryGetValue(branch.Block, out var pos))
                {
                    var offset = pos - branch.Pos - 2;
                    list.WriteAt(branch.Pos, (Int16)offset);
                    continue;
                }

                throw new Exception("Invalid Branch");
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
