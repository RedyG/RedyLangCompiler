using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.ParseTree
{
    public interface Decl
    {
        public ModuleFile ModuleFile { get; }
    }
}
