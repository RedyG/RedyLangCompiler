using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.AST
{
    public struct ImportedFunc
    {
        public string FileName { get; set; }
        public Func Func { get; set; }

        public ImportedFunc(string fileName, Func func)
        {
            FileName = fileName;
            Func = func;
        }
    }

    public class Module
    {
        public List<ImportedFunc> ImportedFuncs { get; set; } = new();
        public List<Func> Funcs { get; set; } = new();

        public ByteCode.Module CodeGen()
        {
            var module = new ByteCode.Module(ImportedFuncs.Select(func => func.FileName).ToList());
            var funcIds = new Dictionary<Func, int>();
            int funcId = 0;
            for (; funcId < ImportedFuncs.Count; funcId++)
                funcIds.Add(ImportedFuncs[funcId].Func, funcId);

            for (int i = 0; i < Funcs.Count; i++)
                funcIds.Add(Funcs[i], i + funcId);

            foreach (var func in Funcs)
                module.Funcs.Add(func.CodeGen(funcIds));

            module.Funcs[0].LastBlock.BrInstruction = ByteCode.BrInstruction.CreateExit();

            return module;
        }

        public Module(List<Func> funcs, List<ImportedFunc> importedFuncs)
        {
            Funcs = funcs;
            ImportedFuncs = importedFuncs;
        }
    }
}
