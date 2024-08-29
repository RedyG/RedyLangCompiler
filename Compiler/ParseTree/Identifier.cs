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

        TypedExpr? IExpr.ToAST()
        {
            throw new NotImplementedException();
        }
    }
}
