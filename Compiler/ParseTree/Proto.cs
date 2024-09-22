using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.ParseTree
{
    public class Proto
    {
        public VisibilityNode VisibilityNode { get; }
        public Type? ReturnType { get; }
        public Identifier Identifier { get; }
        public List<VarDeclStatement> Params { get; }

        public Proto(VisibilityNode visibilityNode, Identifier identifier, List<VarDeclStatement> @params, Type? returnType)
        {
            VisibilityNode = visibilityNode;
            ReturnType = returnType;
            Identifier = identifier;
            Params = @params;
        }

        public AST.Proto? ToAST(Func func, GlobalSymbols globals, ScopedSymbols scopedSymbols)
        {
            var @params = Params.Select(param => param.ToAST(func, globals, scopedSymbols) as AST.VarDeclStatement).ToList();

            if (@params.Any(p => p == null))
                return null;

            if (ReturnType == null)
                return new AST.Proto(new AST.Type.Void(), @params);

            var returnType = ReturnType.ToAST(func.ModuleFile.Module);
            if (returnType == null)
                return null;
            
            return new AST.Proto(returnType, @params);
        }
    }
}
