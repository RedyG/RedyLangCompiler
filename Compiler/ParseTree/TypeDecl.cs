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
        public Attributes? Attributes { get; }
        public VisibilityNode VisibilityNode { get; }
        public Identifier Identifier { get; }
        public List<GenericParam> GenericParams { get; } = new();
        public Type Type { get; }

        public TypeDecl(ModuleFile moduleFile, VisibilityNode visibilityNode, Attributes? attributes, Identifier identifier, Type type, bool isAlias, List<GenericParam>? genericParams = null)
        {
            Attributes = attributes;
            ModuleFile = moduleFile;
            VisibilityNode = visibilityNode;
            Identifier = identifier;
            Type = type;
            IsAlias = isAlias;
            GenericParams = genericParams ?? new List<GenericParam>();
        }

        public AST.IType? GetType(GlobalSymbols globals, ScopedSymbols scopedSymbols)
        {
            var type = Type.ToAST(this, globals, scopedSymbols);
            return IsAlias ? type : new AST.IType.Identifier { Name = Identifier.Name.ToString(), Type = type };
        }

    }
}
