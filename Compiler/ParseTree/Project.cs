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
        public List<ImplDecl> Impls { get; } = new();

        public AST.Project? ToAST()
        {
            var project = new AST.Project();
            var globals = new ParseTree.GlobalSymbols();
            globals.Project = project;

            foreach (var module in Modules.Values)
                foreach (var moduleFile in module.ModuleFiles)
                    moduleFile.ResolveUseDecls();

            foreach (var impl in Impls)
            {
                var implAST = impl.ToAST(globals);
                if (implAST != null)
                    project.Impls.Add(implAST);
            }

            foreach (var module in Modules.Values)
            {
                foreach (var moduleFile in module.ModuleFiles)
                {
                    var moduleAST = moduleFile?.ToAST(globals);
                    if (moduleAST != null)
                        project.Modules.Add(moduleAST);
                }
            }


            return project;
        }
    }
}
