using Compiler.ByteCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.AST
{
    public class VarUseExpr : IExpr
    {
        public IType Type => Var.Type;
        public VarDeclStatement Var { get; set; }

        public VarUseExpr(VarDeclStatement var)
        {
            Var = var;
        }


        public void CodeGen(ByteCode.Func func, Dictionary<Func, ByteCode.Func> funcs, CodeGenSymbols symbols)
        {
            func.LastBlock.Instructions.Add(Instruction.CreateLocalGet((UInt16)symbols.VarIds[Var]));
            if (Var.Refs.Count > 0 && Type.Size() <= 8)
                func.LastBlock.Instructions.Add(Instruction.CreateLoad(Var.Type.Size(), 0));
        }
    }
}
