using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.AST
{
    public class Impl
    {
        public Type Trait { get; }
        public Type Type { get; }
        public List<Func> Funcs { get; }

        public Impl(Type trait, Type type, List<Func> funcs)
        {
            Trait = trait;
            Type = type;
            Funcs = funcs;
        }
    }
}
