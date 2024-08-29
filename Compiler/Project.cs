using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    public class Project
    {
        public Dictionary<string, ParseTree.Module> Modules { get; } = new();
        public Dictionary<string, string> Files { get; } = new();
    }
}
