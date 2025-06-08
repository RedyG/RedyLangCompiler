using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.AST
{
    public class ModuleFile
    {
        public string FileName { get; set; }
        public List<Func> ImportedFuncs { get; set; } = new();
        public List<Func> Funcs { get; set; } = new();
        public List<IType> Types { get; set; } = new();
        public Project Project { get; set; }

        public ByteCode.Module CodeGen(Dictionary<Func, ByteCode.Func> funcSymbols)
        {
            var module = new ByteCode.Module(FileName, [], []);

            module.ImportedFuncs = ImportedFuncs.Where(func => !func.Proto.IsIntrisic).Select(func =>
                func.CodeGen(funcSymbols, module)
            ).ToList();

            module.Funcs = [
                ..Funcs.Where(func => !func.Proto.IsIntrisic).Select(func =>
                    func.CodeGen(funcSymbols, module)
                ),
                ..Project.GetModuleMethods(this).Where(func => !func.Proto.IsIntrisic).Select(func =>
                    func.CodeGen(funcSymbols, module)
                )
            ];

            foreach (var func in module.Funcs)
                func.Module = module;

            return module;
        }

        public ModuleFile(string fileName, List<Func> funcs, List<Func> importedFuncs, List<IType> types, Project project)
        {
            Funcs = funcs;
            ImportedFuncs = importedFuncs;
            FileName = fileName;
            Types = types;
            Project = project;
        }
    }
}
