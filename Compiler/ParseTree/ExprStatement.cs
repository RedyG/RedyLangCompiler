using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.ParseTree
{
    public class ExprStatement : IStatement
    {
        public TextRange Range { get; }
        public IExpr Expr { get; }

        public ExprStatement(TextRange range, IExpr expr)
        {
            Range = range;
            Expr = expr;
        }

        public AST.IStatement? ToAST(Func func, GlobalSymbols globals, ScopedSymbols scopedSymbols)
        {
            var expr = Expr.ToAST(func, globals, scopedSymbols, true);
            if (expr == null)
                return null;

            return new AST.ExprStatement(expr);
        }
    }
}
