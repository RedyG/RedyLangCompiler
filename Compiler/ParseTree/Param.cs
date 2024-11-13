using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.ParseTree
{
    public class Param
    {
        public TextRange Range { get; }
        public Identifier Identifier { get; }
        public Type Type { get; }
        public IExpr? Value { get; }

        public Param(TextRange range, Type type, Identifier identifier, IExpr? value = null)
        {
            Range = range;
            Type = type;
            Identifier = identifier;
            Value = value;
        }

        public AST.Param? Register(Decl decl, GlobalSymbols globals, ScopedSymbols scoped)
        {
            AST.IType? type = Type.ToAST(decl, globals, scoped);
            if (type == null)
                return null;

            var param = new AST.Param(type, null, Value != null);
            globals.ParamsAST.Add(this, param);
            return param;
        }

        public AST.Param? ToAST(Decl decl, GlobalSymbols globals, ScopedSymbols scopedSymbols, AST.Param param)
        {
            scopedSymbols.VarDecls.Add(Identifier.Name, param);
            if (Value == null)
                return param;

            var value = Value.ToAST(decl, globals, scopedSymbols);
            if (value == null)
                return null;

            param.Value = value;
            return param;

        }
    }
}
