using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.AST
{
    public class Field
    {
        public Type.Struct Struct { get; set; }
        public string Name { get; set; }
        public Type Type { get; set; }

        public Field(string name, Type type, Type.Struct @struct)
        {
            Name = name;
            Type = type;
            Struct = @struct;
        }

        public int Offset()
        {
            var fields = Struct.Fields;
            var index = fields.IndexOf(this);
            return fields.Take(index).Sum(f => f.Type.Size());
        }
    }
}
