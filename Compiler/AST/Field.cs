using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.AST
{
    public class Field
    {
        public string Name { get; set; }
        public Type Type { get; set; }

        public Field(string name, Type type)
        {
            Name = name;
            Type = type;
        }
    }
}
