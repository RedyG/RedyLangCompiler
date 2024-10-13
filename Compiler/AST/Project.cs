using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.AST
{
    public class Project
    {
        public List<ModuleFile> Modules { get; } = new();

        public Project()
        {
        }
    }
}
