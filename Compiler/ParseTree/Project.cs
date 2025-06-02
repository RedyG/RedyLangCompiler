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

            foreach (var module in Modules.Flatten())
                foreach (var moduleFile in module.ModuleFiles)
                    moduleFile.ResolveUseDecls();

            foreach (var module in Modules.Flatten())
                foreach (var moduleFile in module.ModuleFiles)
                    project.Modules.Add(moduleFile.Register(globals));

            foreach (var (trait, traitAST) in globals.Traits)
                trait.ToAST(globals, traitAST);


            foreach (var module in Modules.Flatten())
                foreach (var moduleFile in module.ModuleFiles)
                    moduleFile?.ToAST(globals, moduleFile.Register(globals));

            return project;
        }
    }

    public static class ModulesExtension
    {
        public static ParseTree.Module? GetModule(this Dictionary<StringSegment, ParseTree.Module> modules, List<Identifier> names, int depth = 0)
            => GetModule(modules, names.Select(n => n.Name).ToList(), depth);
        public static ParseTree.Module? GetModule(this Dictionary<StringSegment, ParseTree.Module> modules, List<StringSegment> names, int depth = 0)
        {
            var name = names.ElementAtOrDefault(depth);
            if (!modules.TryGetValue(name, out var module))
                return null;

            if (depth == names.Count - 1)
                return module;

            return module.Modules.GetModule(names, depth + 1);
        }

        public static ParseTree.Module CreateModule(this Dictionary<StringSegment, ParseTree.Module> modules, Project project, List<Identifier> names, int depth = 0)
            => CreateModule(modules, project, names.Select(n => n.Name).ToList(), depth);

        public static ParseTree.Module CreateModule(this Dictionary<StringSegment, ParseTree.Module> modules, Project project, List<StringSegment> names, int depth = 0)
        {
            var name = names.ElementAtOrDefault(depth);
            if (!modules.TryGetValue(name, out var module))
            {
                module = new ParseTree.Module(project, name);
                modules.Add(name, module);
            }

            if (depth == names.Count - 1)
                return module;

            return module.Modules.CreateModule(project, names, depth + 1);
        }

        public static IEnumerable<ParseTree.Module> Flatten(this Dictionary<StringSegment, ParseTree.Module> modules)
        {
            foreach (var module in modules.Values)
            {
                yield return module;

                foreach (var subModule in module.Modules.Flatten())
                    yield return subModule;
            }
        }
    }
}
