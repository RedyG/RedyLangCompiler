using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.ParseTree
{
    public class IntExpr : IExpr
    {
        public TextRange Range { get; }
        public long Value { get; }

        public IntExpr(TextRange range, long value)
        {
            Range = range;
            Value = value;
        }

        TypedExpr? IExpr.ToAST() => new TypedExpr(BuiltInTypes.I32, new AST.IntExpr(Value));
    }
}
