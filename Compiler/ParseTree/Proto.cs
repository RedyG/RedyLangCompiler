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
        public VisibilityNode VisibilityNode { get; }
        public Type? ReturnType { get; }
        public Identifier Identifier { get; }
        public List<VarDeclStatement> Params { get; }

        public Proto( VisibilityNode visibilityNode, Identifier identifier, List<VarDeclStatement> @params, Type? returnType, ModuleFile moduleFile)
        {
            VisibilityNode = visibilityNode;
            ReturnType = returnType;
            Identifier = identifier;
            Params = @params;
            ModuleFile = moduleFile;
        }

        public AST.Proto? ToAST(Decl decl, GlobalSymbols globals, ScopedSymbols scopedSymbols)
        {
            var @params = Params.Select(param => param.ToAST(decl, globals, scopedSymbols) as AST.VarDeclStatement).ToList();

            if (@params.Any(p => p == null))
                return null;

            if (ReturnType == null)
                return new AST.Proto(new AST.Type.Void(), @params, Identifier.Name.ToString());

            var returnType = ReturnType.ToAST(decl, globals, scopedSymbols);
            if (returnType == null)
            {
                Logger.TypeNotFound(decl.ModuleFile, ((Type.Identifier)ReturnType).Identifer);
                return null;
            }
            
            return new AST.Proto(returnType, @params, Identifier.Name.ToString());
        }
    }
}
