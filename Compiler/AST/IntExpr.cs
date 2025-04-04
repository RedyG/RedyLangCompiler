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
        public IType Type => new IType.I32();
        public long Value { get; set; }

        public IntExpr(long value)
        {
            Value = value;
        }

        public void CodeGen(ByteCode.Func func, Dictionary<Func, ByteCode.Func> funcs, CodeGenSymbols symbols)
        {
            func.LastBlock.Instructions.Add(Instruction.CreateConst(Value));
        }
    }
}
