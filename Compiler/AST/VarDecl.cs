using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compiler.ByteCode;

namespace Compiler.AST
{
    public class VarDecl : IStatement
    {
        public IExpr Value { get; set; }

        public VarDecl(IExpr value)
        {
            Value = value;
        }

        public void CodeGen(ByteCode.Func func, CodeGenContext ctx)
        {
            ctx.VarIds.Add(this, ctx.CurrentVarId);
            func.LastBlock.Instructions.Add(Instruction.CreateLocalSet((UInt16)ctx.CurrentVarId));
            ctx.CurrentVarId++;
        }
    }
}
