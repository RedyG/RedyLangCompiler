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

            foreach (var useDecl in UseDecls)
            {
                var module = Module.Project.Modules[useDecl.Path[0].Name];

                foreach (var import in useDecl.Imported)
                {
                    var func = module.GetPubFunc(import);
                    if (func != null)
                    {
                        UsedFuncs.Add(import.Name, func);
                        continue;
                    }
                }
            }

            resolved = true;
        }

        private static List<AST.Func> ToAST(Dictionary<StringSegment, Func> funcs, GlobalSymbols globals) => funcs.Values
            .Select(func => func.ToAST(globals))
            .Where(func => func != null)
            .ToList()!;

        private List<AST.Type> ToAST(Dictionary<StringSegment, TypeDecl> typeDecls, GlobalSymbols globals) => typeDecls.Values
            .Select(typeDecl => typeDecl.Type.ToAST(Module))
            .Where(typeDecl => typeDecl != null)
            .ToList()!;

        public AST.ModuleFile ToAST(GlobalSymbols globals) => new AST.ModuleFile(FileName, ToAST(Funcs, globals), ToAST(UsedFuncs, globals), ToAST(TypeDecls, globals));


        public ModuleFile(Module module, string fileName)
        {
            Module = module;
            FileName = fileName;
        }
    }
}
