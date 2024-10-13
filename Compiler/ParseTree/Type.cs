using Compiler.AST;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.ParseTree
{
    public record Type
    {
        public record Struct(List<Field> Fields, TextRange Range) : Type;
        public record Identifier(ParseTree.Identifier Identifer) : Type;
        public record Trait(List<Func> Funcs, List<Proto> Protos, TextRange Range) : Type;

        public AST.Type? ToAST(Module module)
        {
            switch (this)
            {
                case Struct @struct:
                    var structAST = new AST.Type.Struct();
                    var fields = @struct.fields.Select(f => new AST.Field(f.VarDecl.Identifier.Name.ToString(), f.VarDecl.Type.ToAST(module), structAST)).ToList();

                    if (Enumerable.Any<AST.Field>(fields, (Func<AST.Field, bool>)(f => f.Type == null)))
                        return null;

                    structAST.Fields = fields;
                    return structAST;
                case Identifier identifier:
                    switch (identifier.Identifer.Name.ToString())
                    {
                        case "i32":
                            return new AST.Type.I32();
                        case "void":
                            return new AST.Type.Void();
                        default:
                            var type = module.GetType(identifier.Identifer);
                            return type == null ? null : type.ToAST(module);
                    }
                default:
                    return null;
            }
        }


        public TextRange GetRange() => this switch
        {
            Struct s => s.Range,
            Identifier i => i.Identifer.Range,
            Trait t => t.Range,
            _ => throw new NotImplementedException()
        };
    }
}
