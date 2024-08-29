using Compiler.ByteCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.AST
{
    public class IntExpr : IExpr
    {
        public Type Type => new Type.I32();
        public long Value { get; set; }

        public IntExpr(long value)
        {
            Value = value;
        }

        public void CodeGen(ByteCode.Func func, Dictionary<Func, int> funcIds, CodeGenSymbols symbols)
        {
            if (Value < sbyte.MaxValue)
                func.LastBlock.Instructions.Add(Instruction.CreateI8Const((sbyte)Value));
            else if (Value < Int16.MaxValue)
                func.LastBlock.Instructions.Add(Instruction.CreateI16Const((Int16)Value));
            else if (Value < Int32.MaxValue)
                func.LastBlock.Instructions.Add(Instruction.CreateI32Const((Int32)Value));
            else if (Value < Int64.MaxValue)
                func.LastBlock.Instructions.Add(Instruction.CreateI64Const((Int64)Value));
            else
                throw new ArgumentOutOfRangeException();
        }
    }
}
