using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.ParseTree
{
    public class BlockExpr : IExpr
    {
        public TextRange Range { get; }
        public List<IStatement> Statements { get; }
        public IExpr? LastExpr { get; }

        public AST.IExpr? ToAST(Decl decl, GlobalSymbols globals, ScopedSymbols scopedSymbols, bool ignored = false)
        {
            var statements = Statements.Select(s =>
            s.ToAST(decl, globals, scopedSymbols)
            ).ToList();

            if (statements.Any(s => s == null))
                return null;

            if (LastExpr == null)
                return new AST.BlockExpr(new AST.IType.Void(), statements, null);

            var lastExpr = LastExpr.ToAST(decl, globals, scopedSymbols);
            if (lastExpr == null)
                return null;

            return new AST.BlockExpr(lastExpr.Type, statements, lastExpr);
        }

        public BlockExpr(TextRange range, List<IStatement> statements, IExpr? lastExpr = null)
        {
            Range = range;
            Statements = statements;
            LastExpr = lastExpr;
        }
    }
}
