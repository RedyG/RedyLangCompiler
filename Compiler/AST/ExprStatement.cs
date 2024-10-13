using Compiler.ByteCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.AST
{
    public class ExprStatement : IStatement
    {
        public IExpr Expr { get; }

        public ExprStatement(IExpr expr)
        {
            Expr = expr;
        }

        public void CodeGen(ByteCode.Func func, Dictionary<Func, ByteCode.Func> funcs, CodeGenSymbols symbols)
        {
            Expr.CodeGen(func, funcs, symbols);
            if (!Expr.Type.IsEmpty)
                func.LastBlock.Instructions.Add(new Instruction(OpCode.Pop));
        }
    }
}
