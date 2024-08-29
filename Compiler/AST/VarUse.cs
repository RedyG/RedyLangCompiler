using Compiler.ByteCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.AST
{
    public class VarUse : IExpr
    {
        public Type Type => Var.Type;
        public VarDeclStatement Var { get; set; }

        public VarUse(VarDeclStatement var)
        {
            Var = var;
        }


        public void CodeGen(ByteCode.Func func, Dictionary<Func, int> funcIds, CodeGenSymbols symbols)
        {
            func.LastBlock.Instructions.Add(Instruction.CreateLocalGet((UInt16)symbols.VarIds[Var]));
        }
    }
}
