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

        public AST.IExpr? ToAST(Decl decl, GlobalSymbols globals, ScopedSymbols scopedSymbols, bool ignored = false)
        {
            if (decl is Func func)
            {
                var funcAST = globals.FuncsAST[func];
                if (funcAST == null)
                    return null;

                if (Expr == null)
                {
                    if (funcAST.Proto.ReturnType is not AST.IType.Void)
                    {
                        Logger.MismatchedTypesReturnVoid(func.Proto.ModuleFile, func, funcAST.Proto.ReturnType, this);
                        return null;
                    }

                    return new AST.ReturnExpr(null);
                }

                var expr = Expr.ToAST(decl, globals, scopedSymbols);
                if (expr == null)
                    return null;

                return new AST.ReturnExpr(expr);
            }

            throw new Exception("ReturnExpr.ToAST called on a non-func decl");
        }
    }
}
