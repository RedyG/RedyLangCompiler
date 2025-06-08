using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.ParseTree
{
    public class StringExpr : IExpr
    {
        public TextRange Range { get; }
        public StringSegment Content { get; }

        public AST.IExpr? ToAST(Decl decl, GlobalSymbols globals, ScopedSymbols scopedSymbols, bool ignored = false) => new AST.StringExpr(Content.ToString());

        public StringExpr(TextRange range, StringSegment content)
        {
            Range = range;
            Content = content;
        }
    }
}
