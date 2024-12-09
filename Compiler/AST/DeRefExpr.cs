using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.AST
{
    public class DeRefExpr : IExpr
    {
        public IType Type { get; }

        public IExpr Expr { get; }

        public DeRefExpr(IType type, IExpr expr)
        {
            Type = type;
            Expr = expr;
        }

        public void CodeGen(ByteCode.Func func, Dictionary<Func, ByteCode.Func> funcs, CodeGenSymbols symbols)
        {
            throw new NotImplementedException();
        }
    }
}
