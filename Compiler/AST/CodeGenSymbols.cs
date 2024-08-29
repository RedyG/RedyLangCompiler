using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.AST
{
    public class CodeGenSymbols
    {
        public int CurrentVarId { get; private set; } = 0;
        public Dictionary<VarDeclStatement, int> VarIds { get; } = new();

        public void AddVar(VarDeclStatement var)
        {
            VarIds.Add(var, CurrentVarId++);
        }
    }
}
