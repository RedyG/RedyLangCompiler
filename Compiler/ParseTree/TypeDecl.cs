using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.ParseTree
{
    public class TypeDecl : Decl
    {
        public bool IsAlias { get; }
        public ModuleFile ModuleFile { get; }
        public VisibilityNode VisibilityNode { get; }
        public Identifier Identifier { get; }
        public Type Type { get; }

        public TypeDecl(ModuleFile moduleFile, VisibilityNode visibilityNode, Identifier identifier, Type type, bool isAlias)
        {
            ModuleFile = moduleFile;
            VisibilityNode = visibilityNode;
            Identifier = identifier;
            Type = type;
            IsAlias = isAlias;
        }

        public AST.IType? GetType(GlobalSymbols globals, ScopedSymbols scopedSymbols)
        {
            var type = Type.ToAST(this, globals, scopedSymbols);
            return IsAlias ? type : new AST.IType.Identifier { Name = Identifier.Name.ToString(), Type = type };
        }

    }
}
