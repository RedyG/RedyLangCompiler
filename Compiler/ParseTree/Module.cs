﻿using Microsoft.Extensions.Primitives;
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

                if (fileModule.ImportedFuncs.TryGetValue(identifier.Name, out func))
                {
                     if (func.Proto.VisibilityNode.Visibility == Visibility.Pub)
                        return func;

                    if (error)
                    {
                        Logger.FuncPrivate(fileModule, identifier, func);
                        return null;
                    }
                }

                foreach (var module in fileModule.ImportedModules.Values)
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

                if (moduleFile.ImportedFuncs.TryGetValue(identifier.Name, out func))
                {
                    if (func.Proto.VisibilityNode.Visibility == Visibility.Pub)
                        return func;

                    if (error)
                    {
                        Logger.FuncPrivate(moduleFile, identifier, func);
                        return null;
                    }
                }

                foreach (var module in moduleFile.ImportedModules.Values)
                {
                    func = module.GetPubFunc(identifier, false);
                    if (func != null)
                        return func;
                }

            }

            return null;
        }

        public Type? GetType(Identifier identifier, bool error = true)
        {
            foreach (var moduleFile in ModuleFiles)
            {
                if (moduleFile.TypeDecls.TryGetValue(identifier.Name, out var typeDecl))
                    return typeDecl.Type;

                if (moduleFile.ImportedTypeDecls.TryGetValue(identifier.Name, out typeDecl))
                {
                    if (typeDecl.VisibilityNode.Visibility == Visibility.Pub)
                        return typeDecl.Type;

                    if (error)
                    {
                        Logger.TypePrivate(moduleFile, identifier, typeDecl);
                        return null;
                    }
                }

                foreach (var module in moduleFile.ImportedModules.Values)
                {
                    var type = module.GetPubType(identifier, false);
                    if (type != null)
                        return type;
                }
            }

            return null;
        }

        public Type? GetPubType(Identifier identifier, bool error = true)
        {
            foreach (var moduleFile in ModuleFiles)
            {
                if (moduleFile.TypeDecls.TryGetValue(identifier.Name, out var typeDecl))
                {
                    if (typeDecl.VisibilityNode.Visibility == Visibility.Pub)
                        return typeDecl.Type;

                    if (error)
                    {
                        Logger.TypePrivate(moduleFile, identifier, typeDecl);
                        return null;
                    }
                }

                if (moduleFile.ImportedTypeDecls.TryGetValue(identifier.Name, out typeDecl))
                {
                    if (typeDecl.VisibilityNode.Visibility == Visibility.Pub)
                        return typeDecl.Type;

                    if (error)
                    {
                        Logger.TypePrivate(moduleFile, identifier, typeDecl);
                        return null;
                    }
                }

                foreach (var module in moduleFile.ImportedModules.Values)
                {
                    var type = module.GetPubType(identifier, false);
                    if (type != null)
                        return type;
                }
            }

            return null;
        }


        public AST.Module ToAST(GlobalSymbols globals)
        {
            List<AST.Func> funcs = new();
            foreach (var moduleFile in ModuleFiles)
            {
                foreach (var func in moduleFile.Funcs.Values)
                {
                    var funcAST = func?.ToAST(globals);
                    if (funcAST != null)
                        funcs.Add(funcAST);
                }
            }

            return new AST.Module(funcs);
        }

    }
}