using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.ParseTree
{
    public class VarDeclStatement : IStatement
    {
        public TextRange Range { get; }
        public Type Type { get; }
        public Identifier Identifier { get; }
        public IExpr? Value { get; }

        public VarDeclStatement(TextRange range, Type type, Identifier identifier, IExpr? value = null)
        {
            Range = range;
            Type = type;
            Identifier = identifier;
            Value = value;
        }


        public AST.IStatement? ToAST()
        {
            throw new NotImplementedException();
        }
    }
}
