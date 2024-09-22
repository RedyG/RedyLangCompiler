using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.ParseTree
{
    public class Field
    {
        public VisibilityNode VisibilityNode { get; }
        public VarDeclStatement VarDecl { get; }

        public Field(VisibilityNode visibilityNode, VarDeclStatement varDecl)
        {
            VisibilityNode = visibilityNode;
            VarDecl = varDecl;
        }
    }
}
