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

        public TypedExpr? ToAST()
        {
            foreach (var statement in Statements)
            {
                statement.ToAST();
            }

            if (LastExpr == null)
            {
                return new TypedExpr(Type.Void, )
            }
            return LastExpr.ToAST();

            // ?int
            // ?ref int
        }
    }
}
