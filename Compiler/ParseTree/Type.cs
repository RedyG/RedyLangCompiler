using Compiler.AST;
using Compiler.ByteCode;
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
        public record FuncPtr(List<VarDeclStatement> Params, Type ReturnType) : Type;

        public AST.Type? ToAST(Decl decl, GlobalSymbols globals, ScopedSymbols scopedSymbols)
        {
            if (globals.TypesAST.TryGetValue(this, out var typeAST))
                return typeAST;
            switch (this)
            {
                case Struct @struct:
                    var structAST = new AST.Type.Struct();
                    var fields = @struct.Fields.Select(f => new AST.Field(f.VarDecl.Identifier.Name.ToString(), f.VarDecl.Type.ToAST(decl, globals, scopedSymbols), structAST)).ToList();

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
                            var typeDecl = decl.ModuleFile.Module.GetType(identifier.Identifer);
                            if (typeDecl == null)
                                return null;

                            if (typeDecl.IsAlias)
                                return typeDecl.Type.ToAST(decl, globals, new());

                            var identifierAST = new AST.Type.Identifier();
                            identifierAST.Name = identifier.Identifer.Name.ToString();
                            globals.TypesAST.Add(this, identifierAST);
                            identifierAST.Type = typeDecl.Type.ToAST(decl, globals, new());
                            return identifierAST;
                    }
                case FuncPtr funcPtr:
                    var returnType = funcPtr.ReturnType.ToAST(decl, globals, scopedSymbols);
                    if (returnType == null)
                        return null;

                    var @params = funcPtr.Params.Select(param => param.ToAST(decl, globals, scopedSymbols) as AST.VarDeclStatement).ToList();

                    if (@params.Any(p => p.Type == null))
                                           return null;

                    return new AST.Type.FuncPtr(@params, returnType);
                case Trait trait:   
                    var traitAST = new AST.Type.Trait();
                    globals.TypesAST.Add(trait, traitAST);
                    traitAST.Funcs = trait.Funcs.Select(func => func.ToAST(globals)).ToList();
                    traitAST.Protos = trait.Protos.Select(proto => proto.ToAST(decl, globals, new())).ToList();

                    if (traitAST.Funcs.Any(func => func == null) || traitAST.Protos.Any(proto => proto == null))
                        return null;

                    return traitAST;
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
