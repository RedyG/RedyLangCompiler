using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compiler.ByteCode;

namespace Compiler.AST
{
    public class VarAssignExpr : IExpr
    {
        public Type Type => VarDecl.Type;
        public VarDeclStatement VarDecl { get; set; }
        public IExpr Value { get; set; }

        public VarAssignExpr(VarDeclStatement decl, IExpr value)
        {
            VarDecl = decl;
            Value = value;
        }

        public void CodeGen(ByteCode.Func func, Dictionary<Func, ByteCode.Func> funcs, CodeGenSymbols symbols)
        {
            Value.CodeGen(func, funcs, symbols);
            func.LastBlock.Instructions.Add(Instruction.CreateLocalSet((UInt16)symbols.VarIds[VarDecl]));
        }
    }
}
