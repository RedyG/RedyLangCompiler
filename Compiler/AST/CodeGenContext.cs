using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.AST
{
    public record CodeGenContext
    {
        public int CurrentVarId { get; set; } = 0;
        public Dictionary<VarDecl, int> VarIds { get; } = new();
    }
}
