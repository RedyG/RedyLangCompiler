using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.ParseTree
{
    public class ParenExpr : IExpr
    {
        public TextRange Range { get; }
        public IExpr Expr { get; }

        public ParenExpr(TextRange range, IExpr expr)
        {
            Range = range;
            Expr = expr;
        }

        public TypedExpr? ToAST() => Expr.ToAST();
    }
}
