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
        public List<Type> Types { get; set; } = new();

        public ByteCode.Module CodeGen(Dictionary<Func, ByteCode.Func> funcSymbols)
        {
            var importedFuncs = ImportedFuncs.Select(func => func.CodeGen(funcSymbols)).ToList();
            var funcs = Funcs.Select(func => func.CodeGen(funcSymbols)).ToList();
            var module = new ByteCode.Module(FileName, importedFuncs, funcs);
            foreach (var func in module.Funcs)
                func.Module = module;

            return module;
        }

        public ModuleFile(string fileName, List<Func> funcs, List<Func> importedFuncs, List<Type> types)
        {
            Funcs = funcs;
            ImportedFuncs = importedFuncs;
            FileName = fileName;
            Types = types;
        }
    }
}
