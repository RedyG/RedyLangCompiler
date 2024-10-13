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

        public Proto(Type returnType)
        {
            ReturnType = returnType;
        }

        public Proto(Type returnType, List<VarDeclStatement> @params, ModuleFile? module = null)
        {
            ReturnType = returnType;
            Params = @params;
            Module = module;
        }
    }
}
