using Compiler.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.ParseTree
{
    public class Proto
    {
        public ModuleFile ModuleFile { get; }
        public Attributes? Attributes { get; } = null;
        public VisibilityNode VisibilityNode { get; }
        public Type? ReturnType { get; }
        public Identifier Identifier { get; }
        public List<Param> Params { get; }

        public Proto(VisibilityNode visibilityNode, Attributes? attributes, Identifier identifier, List<Param> @params, Type? returnType, ModuleFile moduleFile)
        {
            Attributes = attributes;
            VisibilityNode = visibilityNode;
            ReturnType = returnType;
            Identifier = identifier;
            Params = @params;
            ModuleFile = moduleFile;
        }

        public AST.Proto? Register(Decl decl, GlobalSymbols globals, AST.ModuleFile? moduleFile)
        {
            var @params = Params.Select(param => param.Register(decl, globals, new())).ToList();

            if (@params.Any(p => p == null))
                return null;

            var attributes = Attributes?.Identifiers.Select(attr => attr.Name.ToString()).ToList() ?? [];

            if (ReturnType == null)
                return new AST.Proto(new AST.IType.Void(), @params, attributes, Identifier.Name.ToString(), moduleFile);

            var returnType = ReturnType.ToAST(decl, globals, new());
            if (returnType == null)
            {
                Logger.TypeNotFound(decl.ModuleFile, ((Type.Identifier)ReturnType).Identifer);
                return null;
            }
            
            return new AST.Proto(returnType, @params, attributes, Identifier.Name.ToString(), moduleFile);
        }
        public AST.Proto? ToAST(Decl decl, GlobalSymbols globals, ScopedSymbols scopedSymbols, AST.Proto proto)
        {
            foreach (var param in Params)
            {
                var paramAST = globals.ParamsAST[param];
                if (paramAST == null)
                    return null;

                param.ToAST(decl, globals, scopedSymbols, paramAST);
            }
            return proto;
        }
    }

}
