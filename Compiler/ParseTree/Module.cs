using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.ParseTree
{
    public class Module
    {
        public StringSegment Name = StringSegment.Empty;
        public List<ModuleFile> ModuleFiles = new();
        public Dictionary<StringSegment, ParseTree.Module> Modules { get; } = new();

        public Project Project { get; }

        public Module(Project project, StringSegment name)
        {
            Project = project;
            Name = name;
        }

        public Module(Project project)
        {
            Project = project;
        }

        public Func? GetFunc(Identifier identifier, bool error = true)
        {
            foreach (var fileModule in ModuleFiles)
            {
                if (fileModule.Funcs.TryGetValue(identifier.Name, out var func))
                    return func;

                if (fileModule.UsedFuncs.TryGetValue(identifier.Name, out func))
                {
                     if (func.Proto.VisibilityNode.Visibility == Visibility.Pub)
                        return func;

                    if (error)
                    {
                        Logger.FuncPrivate(fileModule, identifier, func);
                        return null;
                    }
                }

                foreach (var module in fileModule.UsedModules.Values)
                {
                    func = module.GetPubFunc(identifier, false);
                    if (func != null)
                        return func;
                }
            }

            return null;
        }

        public Func? GetPubFunc(Identifier identifier, bool error = true)
        {
            foreach (var moduleFile in ModuleFiles)
            {
                if (moduleFile.Funcs.TryGetValue(identifier.Name, out var func))
                {
                    if (func.Proto.VisibilityNode.Visibility == Visibility.Pub)
                        return func;

                    if (error) {
                        Logger.FuncPrivate(moduleFile, identifier, func);
                        return null;
                    }
                }

                if (moduleFile.UsedFuncs.TryGetValue(identifier.Name, out func))
                {
                    if (func.Proto.VisibilityNode.Visibility == Visibility.Pub)
                        return func;

                    if (error)
                    {
                        Logger.FuncPrivate(moduleFile, identifier, func);
                        return null;
                    }
                }

                foreach (var module in moduleFile.UsedModules.Values)
                {
                    func = module.GetPubFunc(identifier, false);
                    if (func != null)
                        return func;
                }

            }

            return null;
        }

        public TypeDecl? GetType(Identifier identifier, bool error = true)
        {
            foreach (var moduleFile in ModuleFiles)
            {
                if (moduleFile.TypeDecls.TryGetValue(identifier.Name, out var typeDecl))
                    return typeDecl;

                if (moduleFile.UsedTypeDecls.TryGetValue(identifier.Name, out typeDecl))
                {
                    if (typeDecl.VisibilityNode.Visibility == Visibility.Pub)
                        return typeDecl;

                    if (error)
                    {
                        Logger.TypePrivate(moduleFile, identifier, typeDecl);
                        return null;
                    }
                }

                foreach (var module in moduleFile.UsedModules.Values)
                {
                    var type = module.GetPubType(identifier, false);
                    if (type != null)
                        return type;
                }
            }

            return null;
        }

        public TypeDecl? GetPubType(Identifier identifier, bool error = true)
        {
            foreach (var moduleFile in ModuleFiles)
            {
                if (moduleFile.TypeDecls.TryGetValue(identifier.Name, out var typeDecl))
                {
                    if (typeDecl.VisibilityNode.Visibility == Visibility.Pub)
                        return typeDecl;

                    if (error)
                    {
                        Logger.TypePrivate(moduleFile, identifier, typeDecl);
                        return null;
                    }
                }

                if (moduleFile.UsedTypeDecls.TryGetValue(identifier.Name, out typeDecl))
                {
                    if (typeDecl.VisibilityNode.Visibility == Visibility.Pub)
                        return typeDecl;

                    if (error)
                    {
                        Logger.TypePrivate(moduleFile, identifier, typeDecl);
                        return null;
                    }
                }

                foreach (var module in moduleFile.UsedModules.Values)
                {
                    var type = module.GetPubType(identifier, false);
                    if (type != null)
                        return type;
                }
            }

            return null;
        }



    }
}
