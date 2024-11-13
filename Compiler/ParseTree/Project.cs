using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.ParseTree
{
    public class Project
    {
        public Dictionary<StringSegment, ParseTree.Module> Modules { get; } = new();
        public Dictionary<string, string> Files { get; } = new();

        public AST.Project? ToAST()
        {
            var project = new AST.Project();
            var globals = new ParseTree.GlobalSymbols();
            globals.Project = project;

            foreach (var module in Modules.Values)
                foreach (var moduleFile in module.ModuleFiles)
                    moduleFile.ResolveUseDecls();

            foreach (var module in Modules.Values)
                foreach (var moduleFile in module.ModuleFiles)
                    project.Modules.Add(moduleFile.Register(globals));

            foreach (var (trait, traitAST) in globals.Traits)
                trait.ToAST(globals, traitAST);


            foreach (var module in Modules.Values)
                foreach (var moduleFile in module.ModuleFiles)
                    moduleFile?.ToAST(globals, moduleFile.Register(globals));

            return project;
        }
    }
}
