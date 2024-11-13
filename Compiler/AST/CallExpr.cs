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
        public IType Type => Func.Proto.ReturnType;
        public Func Func { get; }
        public List<IExpr> Args { get; } = new();

        public CallExpr(Func func, List<IExpr> args)
        {
            Func = func;
            Args = args;
        }

        public void CodeGen(ByteCode.Func func, Dictionary<Func, ByteCode.Func> funcs, CodeGenSymbols symbols)
        {
            foreach (var arg in Args)
                arg.CodeGen(func, funcs, symbols);

            if (Type.ToConcrete() is IType.Struct @struct)
            {
                func.LastBlock.Instructions.Add(Instruction.CreateAlloca(@struct.Size()));
            }
            func.LastBlock.Instructions.Add(Instruction.CreateCall(Func.CodeGen(funcs)));
        }
    }
}
