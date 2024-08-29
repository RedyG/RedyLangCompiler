using Compiler.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.ParseTree
{
    public record Type
    {
        public static Type I32 = new Identifier(new ParseTree.Identifier(new TextRange(), "i32"));
        public static Type Void = new Identifier(new ParseTree.Identifier(new TextRange(), "void"));

        public record Struct(List<Field> fields, TextRange Range) : Type;
        public record Identifier(ParseTree.Identifier Identifer) : Type;


        public TextRange GetRange() => this switch
        {
            Struct s => s.Range,
            Identifier i => i.Identifer.Range,
            _ => throw new NotImplementedException()
        };
    }
}
