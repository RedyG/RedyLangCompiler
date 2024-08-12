using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compiler.ByteCode;

namespace Compiler.AST
{
    public class VarAssign : IStatement
    {
        public VarDecl VarDecl { get; set; }
        public IExpr Value { get; set; }

        public VarAssign(VarDecl decl, IExpr value)
        {
            VarDecl = decl;
            Value = value;
        }

        public void CodeGen(ByteCode.Func func, CodeGenContext ctx)
        {
            func.LastBlock.Instructions.Add(Instruction.CreateLocalSet((UInt16)ctx.VarIds[VarDecl]));
        }
    }
}
