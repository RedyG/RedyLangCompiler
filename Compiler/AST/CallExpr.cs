using Compiler.ByteCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.AST
{
    public class CallExpr : IExpr
    {
        public Type Type => Func.Proto.ReturnType;
        public Func Func { get; }
        public List<IExpr> Args { get; } = new();

        public CallExpr(Func func, List<IExpr> args)
        {
            Func = func;
            Args = args;
        }

        public void CodeGen(ByteCode.Func func, Dictionary<Func, int> funcIds, CodeGenSymbols symbols)
        {
            foreach (var arg in Args)
                arg.CodeGen(func, funcIds, symbols);
            func.LastBlock.Instructions.Add(Instruction.CreateCall((UInt16)funcIds[Func]));
        }
    }
}
