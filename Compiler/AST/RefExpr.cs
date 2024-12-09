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
            Expr = expr;
        }

        public void CodeGen(ByteCode.Func func, Dictionary<Func, ByteCode.Func> funcs, CodeGenSymbols symbols)
        {
            if (Expr is VarUseExpr varUse)
            {
                varUse.CodeGen(func, funcs, symbols);
                func.LastBlock.Instructions.Pop(); // remove the load instruction, we want the ref
                return;
            }

            throw new Exception("you can only ref vars at the moment"); // TODO: support access to struct fields
        }
    }
}
