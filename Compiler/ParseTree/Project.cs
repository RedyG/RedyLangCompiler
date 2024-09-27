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

        public List<AST.Module>? ToAST()
        {
            List<AST.Module> modules = new();
            var globals = new ParseTree.GlobalSymbols();
            foreach (var module in Modules.Values)
            {
                foreach (var moduleFile in module.ModuleFiles)
                {
                    var moduleAST = moduleFile?.ToAST(globals);
                    if (moduleAST != null)
                        modules.Add(moduleAST);
                }
            }

            return modules;
        }
    }
}
