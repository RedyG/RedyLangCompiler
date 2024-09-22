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

        public Dictionary<StringSegment, TypeDecl> TypeDecls { get; } = new();
        public Dictionary<StringSegment, TypeDecl> ImportedTypeDecls { get; } = new();

        public Dictionary<StringSegment, Func> Funcs { get; } = new();
        public Dictionary<StringSegment, Func> ImportedFuncs { get; } = new();

        public Dictionary<StringSegment, Module> ImportedModules { get; } = new(); 


        public ModuleFile(Module module, string fileName)
        {
            Module = module;
            FileName = fileName;
        }
    }
}
