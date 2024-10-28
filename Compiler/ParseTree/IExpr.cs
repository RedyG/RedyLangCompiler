using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.ParseTree
{
    public interface IExpr : INode
    {
        public AST.IExpr? ToAST(Decl decl, GlobalSymbols globals, ScopedSymbols scopedSymbols, bool ignored = false); // func only for the moment

        public bool IsBlock() => this is BlockExpr _ or IfExpr _;

    }
}
