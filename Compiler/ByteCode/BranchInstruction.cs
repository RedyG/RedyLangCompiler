using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.ByteCode
{
    public enum BranchOpCode
    {
        BrBlock,
        Ret,
        Exit,
    }

    public readonly struct BranchInstruction
    {
        public const byte Br = 0x01;
        public const byte BrTrue = 0x02;
        public const byte BrFalse = 0x03;
        public const byte Exit = 0x04;
        public const byte Ret = 0x07;


        public readonly BranchOpCode OpCode;
        public readonly Block? TrueBlock;
        public readonly Block? FalseBlock;

        private BranchInstruction(BranchOpCode opCode, Block? trueBlock = null, Block? falseBlock = null)
        {
            OpCode = opCode;
            TrueBlock = trueBlock;
            FalseBlock = falseBlock;
        }

        public static BranchInstruction CreateExit() => new BranchInstruction(BranchOpCode.Exit);

        public static BranchInstruction CreateRet() => new BranchInstruction(BranchOpCode.Ret);

        public static BranchInstruction CreateBr(Block trueBlock, Block falseBlock) => new BranchInstruction(BranchOpCode.BrBlock, trueBlock, falseBlock);
    }
}
