using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.ParseTree
{
    public class DeRefExpr : IExpr
    {
        public TextRange Range { get; }
        public IExpr Expr { get; }

        public DeRefExpr(TextRange range, IExpr expr)
        {
            Range = range;
            Expr = expr;
        }

        public AST.IExpr? ToAST(Decl decl, GlobalSymbols globals, ScopedSymbols scopedSymbols, bool ignored = false)
        {
            var expr = Expr.ToAST(decl, globals, scopedSymbols, ignored);
            if (expr == null)
                return null;

            var refType = expr.Type as AST.IType.Ref;
            if (refType == null)
            {
                Logger.ExpectedRefType(decl.ModuleFile, expr.Type, this);
                return null;
            }

            return new AST.DeRefExpr(refType.Type, expr);
        }
    }
}
