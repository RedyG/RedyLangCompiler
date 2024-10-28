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

        public AST.IExpr? ToAST(Decl decl, GlobalSymbols globals, ScopedSymbols scopedSymbols, bool ignored = false)
        {
            if (scopedSymbols.VarDecls.TryGetValue(Name, out var var))
            {
                return new AST.VarUseExpr(var);
            }
            else
            {
                Logger.ValueNotFoundInScope(decl.ModuleFile, this);
                return null;
            }

        }
    }
}
