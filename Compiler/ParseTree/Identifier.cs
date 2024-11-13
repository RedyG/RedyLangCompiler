using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.ParseTree
{
    public struct Identifier : IExpr, IEquatable<Identifier>
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

        public override bool Equals([NotNullWhen(true)] object? obj) => obj is Identifier identifier && Equals(identifier);

        public override int GetHashCode() => HashCode.Combine(Name);

        public bool Equals(Identifier other) =>
            Name == other.Name;
    }
}
