using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.ParseTree
{
    public class RefExpr : IExpr
    {
        public TextRange Range { get; }
        public IExpr Expr { get; }

        public RefExpr(TextRange range, IExpr expr)
        {
            Range = range;
            Expr = expr;
        }

        public AST.IExpr? ToAST(Decl decl, GlobalSymbols globals, ScopedSymbols scopedSymbols, bool ignored = false)
        {
            var expr = Expr.ToAST(decl, globals, scopedSymbols, ignored);
            if (expr == null)
                return null;

            var exprAST = new AST.RefExpr(new AST.IType.Ref(expr.Type), expr);
            if (Expr is Identifier identifier && scopedSymbols.VarDecls.TryGetValue(identifier.Name, out var varDecl))
                varDecl.Refs.Add(exprAST);

            return exprAST;
        }
    }
}
