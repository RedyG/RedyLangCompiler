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
        public List<Impl> Impls { get; } = new();

        public Func? GetMethod(IType type, string name)
        {
            foreach (var impl in Impls)
            {
                if (impl.Type == type)
                {
                    foreach (var method in impl.Funcs)
                    {
                        if (method.Proto.Name == name)
                            return method;
                    }
                }
            }
            return null;
        }

        public List<Func> GetModuleMethods(ModuleFile module) => Impls
            .Where(impl => impl.Module == module)
            .SelectMany(impl => impl.Funcs)
            .ToList();

        public Project()
        {
        }
    }
}
