
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

        public List<ImplDecl> Impls { get; } = new();

        public void ResolveUseDecls()
        {
            if (resolved)
                return;

            foreach (var useDecl in UseDecls)
            {
                var module = Module.Project.Modules.GetModule(useDecl.Path);

                foreach (var import in useDecl.Imported)
                {
                    var func = module.GetPubFunc(import);
                    if (func != null)
                    {
                        UsedFuncs.Add(import.Name, func);
                        continue;
                    }
                    var type = module.GetPubType(import);
                    if (type != null)
                    {
                        UsedTypeDecls.Add(import.Name, type);
                        continue;
                    }
                }
            }

            resolved = true;
        }

        public AST.ModuleFile Register(GlobalSymbols globals)
        {
            if (globals.ModuleFiles.TryGetValue(this, out var moduleAST))
                return moduleAST;

            moduleAST = new AST.ModuleFile(FileName, [], [], [], globals.Project);
            globals.ModuleFiles.Add(this, moduleAST);

            moduleAST.Funcs = Funcs.Values
                .Select(func => func.Register(globals, moduleAST))
                .Where(func => func != null)
                .ToList()!;

            moduleAST.ImportedFuncs = UsedFuncs.Values
                .Select(func => func.Register(globals, moduleAST))
                .Where(func => func != null)
                .ToList()!;

            moduleAST.Types = TypeDecls.Values
                .Select(typeDecl => typeDecl.Type.ToAST(typeDecl, globals, new()))
                .Where(typeDecl => typeDecl != null)
                .ToList()!;

            // impls
            foreach (var impl in Impls)
            {
                var implAST = impl.Register(globals, moduleAST);
                if (implAST != null)
                    globals.Project.Impls.Add(implAST);
            }

            return moduleAST;
        }

        public void ToAST(GlobalSymbols globals, AST.ModuleFile moduleAST)
        {
            moduleAST.Funcs = moduleAST.Funcs.Select(funcAST => {
                var func = Funcs[funcAST.Proto.Name]; // todo: maybe function overloading
                var newFuncAST = func.ToAST(globals, funcAST);
                if (newFuncAST == null)
                    return null;
                return newFuncAST;
            }).ToList()!;

            moduleAST.ImportedFuncs = moduleAST.ImportedFuncs.Select(funcAST => {
                var func = UsedFuncs[funcAST.Proto.Name]; // todo: maybe function overloading
                var newFuncAST = func.ToAST(globals, funcAST);
                if (newFuncAST == null)
                    return null;
                return newFuncAST;
            }).ToList()!;
            
            foreach (var impl in Impls)
            {
                foreach (var func in impl.Funcs)
                {
                    var funcAST = globals.FuncsAST[func];
                    if (funcAST == null)
                        continue;
                    func.ToAST(globals, funcAST);
                }
            }
        }


        public ModuleFile(Module module, string fileName)
        {
            Module = module;
            FileName = fileName;
        }
    }
}
