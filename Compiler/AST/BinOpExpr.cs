using Compiler.ByteCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.AST
{
    public class BinOpExpr : IExpr
    {
        public Type Type { get; }
        public IExpr Left { get; }
        public BinOp Op { get; }
        public IExpr Right { get; }

        public BinOpExpr(Type type, IExpr left, BinOp op, IExpr right)
        {
            Type = type;
            Left = left;
            Op = op;
            Right = right;
        }

        private OpCode GetOpCode() => (Type, Op) switch
        {
            (Type.I32, BinOp.Add) => OpCode.I32Add,
            (Type.I32, BinOp.Sub) => OpCode.I32Sub,
            (Type.I32, BinOp.Mul) => OpCode.I32Mul,
            (Type.I32, BinOp.Div) => OpCode.I32Div,
            (Type.I32, BinOp.Lt) => OpCode.I32Lt,

            (_, _) => throw new NotImplementedException()
        };

        public void CodeGen(ByteCode.Func func, Dictionary<Func, int> funcIds, CodeGenSymbols symbols)
        {
            Left.CodeGen(func, funcIds, symbols);
            Right.CodeGen(func, funcIds, symbols);
            func.LastBlock.Instructions.Add(new Instruction(GetOpCode()));
        }
    }
}
