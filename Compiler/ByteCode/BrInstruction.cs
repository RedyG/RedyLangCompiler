using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.ByteCode
{
    public enum BrOpCode
    {
        Br,
        BrIf,
        Ret,
        RetVoid,
        Exit,
    }

    public readonly struct BrInstruction
    {
        public const byte Br = 0x01;
        public const byte BrTrue = 0x02;
        public const byte BrFalse = 0x03;
        public const byte Ret = 0x08;
        public const byte RetVoid = 0x09;


        public readonly BrOpCode OpCode;
        public readonly Block? TrueBlock;
        public readonly Block? FalseBlock;

        private BrInstruction(BrOpCode opCode, Block? trueBlock = null, Block? falseBlock = null)
        {
            OpCode = opCode;
            TrueBlock = trueBlock;
            FalseBlock = falseBlock;
        }

        public static BrInstruction CreateExit() => new BrInstruction(BrOpCode.Exit);

        public static BrInstruction CreateRet() => new BrInstruction(BrOpCode.Ret);

        public static BrInstruction CreateRetVoid() => new BrInstruction(BrOpCode.RetVoid);

        public static BrInstruction CreateBr(Block block) => new BrInstruction(BrOpCode.BrIf, block);

        public static BrInstruction CreateBrIf(Block trueBlock, Block falseBlock) => new BrInstruction(BrOpCode.BrIf, trueBlock, falseBlock);
    }
}
