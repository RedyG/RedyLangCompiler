using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.ParseTree
{
    public struct Identifier : IExpr
    {
        public TextRange Range { get; }
        public StringSegment Name { get; }

        public Identifier(TextRange range, StringSegment name)
        {
            Range = range;
            Name = name;
        }

        public AST.IExpr? ToAST(Func func, GlobalSymbols globals, ScopedSymbols scopedSymbols, bool ignored = false)
        {
            if (scopedSymbols.VarDecls.TryGetValue(Name, out var decl))
            {
                return new AST.VarUseExpr(decl);
            }
            else
            {
                Logger.ValueNotFoundInScope(func.Proto.ModuleFile, this);
                return null;
            }

        }
    }
}
