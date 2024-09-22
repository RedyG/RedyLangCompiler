using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.ParseTree
{
    public class GlobalSymbols
    {
        public Dictionary<Func, AST.Func> FuncsAST { get; } = new();
    }

    public class ScopedSymbols
    {
        public Dictionary<StringSegment, AST.VarDeclStatement> VarDecls { get; } = new();
    }
}
