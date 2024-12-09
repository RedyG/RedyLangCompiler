using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.AST
{
    public class Field : IEquatable<Field>
    {
        public IType.Struct Struct { get; set; }
        public string Name { get; set; }
        public IType Type { get; set; }

        public Field(string name, IType type, IType.Struct @struct)
        {
            Name = name;
            Type = type;
            Struct = @struct;
        }

        public uint Offset()
        {
            var fields = Struct.Fields;
            var index = fields.IndexOf(this);
            return (uint)fields.Take(index).Sum(f => f.Type.Size());
        }

        public override bool Equals(object? obj) => Equals(obj as Field);

        public override int GetHashCode() => HashCode.Combine(Name, Type);

        public bool Equals(Field? other)
        {
            if (other == null)
                return false;

            return Name == other.Name && Type.Equals(other.Type);
        }
    }
}
