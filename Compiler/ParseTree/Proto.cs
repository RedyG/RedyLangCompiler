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
        public Type ReturnType { get; }
        public Identifier Identifier { get; }
        public List<VarDeclStatement> Params { get; }

        public Proto(VisibilityNode visibilityNode, Identifier identifier, List<VarDeclStatement> @params, Type returnType)
        {
            VisibilityNode = visibilityNode;
            ReturnType = returnType;
            Identifier = identifier;
            Params = @params;
        }
    }
}
