using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.AST
{
    public record Type
    {
        public record Struct(List<Field> fields) : Type;
        public record Void : Type;
        public record Never : Type;
        public record I32 : Type;
        public record Bool : Type;

        public bool IsEmpty => this is Void || this is Never;

        public sealed override string ToString() => this switch
        {
            Struct s => $"struct {{\n{string.Join(",\n", s.fields.Select(field => $"{field.Name}: {field.Type}"))}}}",
            Void => "void",
            Never => "never",
            I32 => "i32",
            Bool => "bool",
            _ => throw new NotImplementedException()
        };
    }


}
