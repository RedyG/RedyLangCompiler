using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.AST
{
    public record Type
    {
        public record Struct() : Type
        {
            public List<Field> Fields { get; set; } = new List<Field>();
        }
        public record FuncPtr(List<VarDeclStatement> Params, Type ReturnType) : Type;
        public record Trait() : Type
        {
            public List<Func> Funcs { get; set; } = new List<Func>();
            public List<Proto> Protos { get; set; } = new List<Proto>();
        }

        public record Identifier() : Type
        {
            public string Name;
            public Type Type;
        }

        public record Void : Type;
        public record Never : Type;
        public record I32 : Type;
        public record Bool : Type;

        public bool IsEmpty => this is Void || this is Never;

        public Type ToConcrete() => this switch
        {
            Identifier identifier => identifier.Type.ToConcrete(),
            _ => this
        };

        public sealed override string ToString() => this switch
        {
            Struct s => $"struct {{\n{string.Join(",\n", s.Fields.Select(field => $"{field.Name}: {field.Type}"))}}}",
            Void => "void",
            Never => "never",
            I32 => "i32",
            Bool => "bool",
            FuncPtr f => $"fn({string.Join(", ", f.Params.Select(param => param.Type.ToString()))}) -> {f.ReturnType}",
            Trait t => $"trait {{\n{string.Join(",\n", t.Funcs.Select(func => func.ToString()))}}}",
            _ => throw new NotImplementedException()
        };

        public uint Size() => this switch
        {
            Struct s => (uint)s.Fields.Sum(field => field.Type.Size()),
            Trait trait => 16,
            FuncPtr _ => 8,
            I32 => 4,
            Bool => 1,
            _ => 0
        };
    }


}
