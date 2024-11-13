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
        public AST.Project Project { get; set; }
        public Dictionary<Func, AST.Func> FuncsAST { get; } = new();
        public Dictionary<Type, AST.IType> TypesAST { get; } = new();
        public List<(Type.Trait, AST.IType.Trait)> Traits { get; } = new();
        public Dictionary<ModuleFile, AST.ModuleFile> ModuleFiles { get; } = new();
        public Dictionary<Param, AST.Param> ParamsAST { get; } = new();
    }

    public class ScopedSymbols
    {
        public Dictionary<StringSegment, AST.VarDeclStatement> VarDecls { get; } = new();
    }
}
