using Compiler.ByteCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.AST
{
    public class RefExpr : IExpr
    {
        public IType Type { get; }

        public IExpr Expr { get; }

        public RefExpr(IType.Ref type, IExpr expr)
        {
            Type = type;
            Expr = expr;
        }

        public void CodeGen(ByteCode.Func func, Dictionary<Func, ByteCode.Func> funcs, CodeGenSymbols symbols)
        {
            if (Expr is VarUseExpr varUse)
            {
                varUse.CodeGen(func, funcs, symbols);
                //func.LastBlock.Instructions.Pop(); // remove the load instruction, we want the ref
                return;
            }

            if (Expr is AccessExpr access)
            {
                access.LValue.CodeGen(func, funcs, symbols);
                if (access.Field.Offset() != 0) {
                    func.LastBlock.Instructions.Add(Instruction.CreateConst(access.Field.Offset()));
                    func.LastBlock.Instructions.Add(new Instruction(OpCode.I64Add));
                }
                return;
            }

            throw new NotImplementedException($"RefExpr.CodeGen not implemented for {Expr.GetType()}");
        }
    }
}
