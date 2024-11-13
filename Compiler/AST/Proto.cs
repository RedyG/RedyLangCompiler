using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.AST
{
    public class Proto
    {
        public ModuleFile? Module { get; set; }
        public IType ReturnType { get; set; }
        public List<Param> Params { get; set; } = new();
        public string Name;

        public Proto(IType returnType, string name)
        {
            ReturnType = returnType;
            Name = name;
        }

        public Proto(IType returnType, List<Param> @params, string name, ModuleFile? module)
        {
            ReturnType = returnType;
            Name = name;
            Params = @params;
            Module = module;
        }
    }
}
