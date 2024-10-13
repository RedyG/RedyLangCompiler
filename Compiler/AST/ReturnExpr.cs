using Compiler.ByteCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.AST
{
    public class ReturnExpr : IExpr
    {
        public Type Type => new Type.Never();
        public IExpr? Value { get; }

        public ReturnExpr(IExpr? value)
        {
            Value = value;
        }

        public void CodeGen(ByteCode.Func func, Dictionary<Func, ByteCode.Func> funcs, CodeGenSymbols symbols)
        {
            Value?.CodeGen(func, funcs, symbols);
            func.LastBlock.BrInstruction = BrInstruction.CreateRet();
        }
    }
}
