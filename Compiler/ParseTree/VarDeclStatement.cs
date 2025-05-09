﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.ParseTree
{
    public class VarDeclStatement : IStatement
    {
        public TextRange Range { get; }
        public Identifier Identifier { get; }
        public Type? Type { get; }
        public IExpr? Value { get; }

        public VarDeclStatement(TextRange range, Type? type, Identifier identifier, IExpr? value = null)
        {
            Range = range;
            Type = type;
            Identifier = identifier;
            Value = value;
        }

        public AST.Param? Register(Decl decl, GlobalSymbols globals, ScopedSymbols scoped)
        {
            AST.IType? type = null;
            if (Type != null)
            {
                type = Type.ToAST(decl, globals, scoped);
                if (type == null)
                    return null;
            }

            if (type == null)
            {
                Logger.ExpectedTypeOrValueVarDecl(decl.ModuleFile, this);
                return null;
            }

            var param = new AST.Param(type, null, Value != null);
            return param;
        }


        public AST.IStatement? ToAST(Decl decl, GlobalSymbols globals, ScopedSymbols scopedSymbols)
        {
            AST.IType? type = null;
            if (Type != null)
            {
                type = Type.ToAST(decl, globals, scopedSymbols);
                if (type == null)
                    return null;
            }

            AST.IExpr? value = null;
            if (Value != null)
            {
                value = Value.ToAST(decl, globals, scopedSymbols);
                if (value == null)
                    return null;

                if (type == null)
                    type = value.Type;

                if (!type.Equals(value.Type))
                {
                    Logger.MismatchedTypesVarDecl(decl.ModuleFile, (AST.IType)type, (AST.IType)value.Type, this);
                    return null;
                }
            }

            if (type == null)
            {
                Logger.ExpectedTypeOrValueVarDecl(decl.ModuleFile, this);
                return null;
            }

            var varDecl = new AST.VarDeclStatement(type, value);
            scopedSymbols.VarDecls.Add(Identifier.Name, varDecl);
            return varDecl;
        }
    }
}
