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
        public Type ReturnType { get; set; }
        public List<VarDeclStatement> Params { get; set; } = new();
        public string Name;

        public Proto(Type returnType, string name)
        {
            ReturnType = returnType;
            Name = name;
        }

        public Proto(Type returnType, List<VarDeclStatement> @params, string name, ModuleFile? module = null)
        {
            ReturnType = returnType;
            Name = name;
            Params = @params;
            Module = module;
        }
    }
}
