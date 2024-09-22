using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.ParseTree
{
    public class ReturnExpr : IExpr
    {
        public IExpr? Expr { get; }

        public TextRange Range { get; }

        public ReturnExpr(TextRange range, IExpr? expr = null)
        {
            Range = range;
            Expr = expr;
        }

        public AST.IExpr? ToAST(Func func, GlobalSymbols globals, ScopedSymbols scopedSymbols, bool ignored = false)
        {
            var funcAST = func.ToAST(globals);
            if (funcAST == null)
                return null;

            if (Expr == null)
            {
                if (funcAST.Proto.ReturnType is not AST.Type.Void)
                {
                    Logger.MismatchedTypesReturnVoid(func.ModuleFile, func, funcAST.Proto.ReturnType, this);
                    return null;
                }
                    
                return new AST.ReturnExpr(null);
            }

            var expr = Expr.ToAST(func, globals, scopedSymbols);
            if (expr == null)
                return null;

            return new AST.ReturnExpr(expr);
        }
    }
}
