using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.AST
{
    public class Impl
    {
        public IType? Trait { get; }
        public IType Type { get; }
        public List<Func> Funcs { get; }
        public ModuleFile Module { get; }

        public Impl(IType? trait, IType type, List<Func> funcs, ModuleFile module)
        {
            Trait = trait;
            Type = type;
            Funcs = funcs;
            Module = module;
        }
    }
}
