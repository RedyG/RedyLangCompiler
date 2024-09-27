using Compiler.AST;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.ParseTree
{
    public class ModuleFile
    {
        public Module Module { get; }
        public string FileName { get; }

        public List<UseDecl> UseDecls { get; } = new();
        private bool resolved = false;

        public Dictionary<StringSegment, TypeDecl> TypeDecls { get; } = new();
        public Dictionary<StringSegment, TypeDecl> UsedTypeDecls { get; } = new();

        public Dictionary<StringSegment, Func> Funcs { get; } = new();
        public Dictionary<StringSegment, Func> UsedFuncs { get; } = new();

        public Dictionary<StringSegment, Module> UsedModules { get; } = new(); 

        public void ResolveUseDecls()
        {
            if (resolved)
                return;
            resolved = true;
        }

        public AST.Module ToAST(GlobalSymbols globals)
        {
            List<ImportedFunc> importedFuncs = new();
            List<AST.Func> funcs = new();

            foreach (var func in UsedFuncs.Values)
            {
                if (func == null)
                    continue;

                var funcAST = func.ToAST(globals);
                if (funcAST != null)
                    importedFuncs.Add(new ImportedFunc(func.ModuleFile.FileName, funcAST));
            }



            foreach (var func in Funcs.Values)
            {
                var funcAST = func?.ToAST(globals);
                if (funcAST != null)
                    funcs.Add(funcAST);
            }



            return new AST.Module(funcs);
        }


        public ModuleFile(Module module, string fileName)
        {
            Module = module;
            FileName = fileName;
        }
    }
}
