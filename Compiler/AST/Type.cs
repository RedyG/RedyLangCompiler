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
    }
}
