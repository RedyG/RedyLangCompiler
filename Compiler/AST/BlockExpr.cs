using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.AST
{
    public class BlockExpr : IExpr
    {
        public Type Type { get; }
        public List<IStatement> Statements { get; }
        public IExpr? LastExpr { get; }

        public BlockExpr(Type type, List<IStatement> statements, IExpr? lastExpr)
        {
            Type = type;
            Statements = statements;
            LastExpr = lastExpr;
        }

        public void CodeGen(ByteCode.Func func, Dictionary<Func, ByteCode.Func> funcs, CodeGenSymbols symbols)
        {
            foreach (var statement in Statements)
                statement.CodeGen(func, funcs, symbols);

            LastExpr?.CodeGen(func, funcs, symbols);
        }
    }
}
