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
        public record Trait(List<Func> Funcs, List<Proto> Protos, TextRange Range) : Type
        {
            public void ToAST(GlobalSymbols globals, AST.IType.Trait traitAST)
            {
                foreach (var funcAST in traitAST.Funcs)
                {
                    var func = Funcs.FirstOrDefault(f => f.Proto.Identifier.Name == funcAST.Proto.Name);
                    if (func == null)
                        continue;
                    func.ToAST(globals, funcAST);
                }
            }
        }
        public record FuncPtr(List<VarDeclStatement> Params, Type ReturnType) : Type;
        public record Ref(Type Type, TextRange Range) : Type;

        public AST.IType? ToAST(Decl decl, GlobalSymbols globals, ScopedSymbols scopedSymbols)
        {
            if (globals.TypesAST.TryGetValue(this, out var typeAST))
                return typeAST;
            switch (this)
            {
                case Struct @struct:
                    var structAST = new AST.IType.Struct();
                    var fields = @struct.Fields.Select(f => new AST.Field(f.VarDecl.Identifier.Name.ToString(), f.VarDecl.Type.ToAST(decl, globals, scopedSymbols), structAST)).ToList();

                    if (Enumerable.Any<AST.Field>((IEnumerable<AST.Field>)fields, (Func<AST.Field, bool>)(f => f.Type == null)))
                        return null;

                    structAST.Fields = fields;
                    return structAST;
                case Identifier identifier:
                    switch (identifier.Identifer.Name.ToString())
                    {
                        case "i32":
                            return new AST.IType.I32();
                        case "void":
                            return new AST.IType.Void();
                        case "string":
                            return new AST.IType.String();
                        default:
                            var typeDecl = decl.ModuleFile.Module.GetType(identifier.Identifer);
                            if (typeDecl == null)
                                return null;

                            if (typeDecl.IsAlias)
                                return typeDecl.Type.ToAST(decl, globals, new());

                            var identifierAST = new AST.IType.Identifier();
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

                    return new AST.IType.FuncPtr(@params, returnType);
                case Trait trait:   
                    var traitAST = new AST.IType.Trait(); // todo fix this caching stuff
                    globals.TypesAST.Add(trait, traitAST);
                    traitAST.Funcs = trait.Funcs.Select(func => func.Register(globals, null)).ToList()!;
                    traitAST.Protos = trait.Protos.Select(proto => proto.Register(decl, globals, null)).ToList()!;

                    if (traitAST.Funcs.Any(func => func == null) || traitAST.Protos.Any(proto => proto == null))
                        return null;

                    return traitAST;
                case Ref r:
                    var type = r.Type.ToAST(decl, globals, scopedSymbols);
                    if (type == null)
                        return null;

                    return new AST.IType.Ref(type);
                default:
                    return null;
            }
        }


        public TextRange GetRange() => this switch
        {
            Struct s => s.Range,
            Identifier i => i.Identifer.Range,
            Trait t => t.Range,
            Ref r => r.Range,
            _ => throw new NotImplementedException()
        };
    }
}
