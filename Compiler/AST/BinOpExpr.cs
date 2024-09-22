using Compiler.ByteCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BinOp = Compiler.ParseTree.BinOp;

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
            (Type.I32, BinOp.Mul) => OpCode.I32Mul,
            (Type.I32, BinOp.Div) => OpCode.I32Div,
            (Type.I32, BinOp.Add) => OpCode.I32Add,
            (Type.I32, BinOp.Sub) => OpCode.I32Sub,
            (Type.Bool, BinOp.Lt) => OpCode.I32Lt,
            (Type.Bool, BinOp.Le) => OpCode.I32Le,
            (Type.Bool, BinOp.Gt) => OpCode.I32Gt,
            (Type.Bool, BinOp.Ge) => OpCode.I32Ge,

            (_, _) => throw new NotImplementedException()
        };

        public void CodeGen(ByteCode.Func func, Dictionary<Func, int> funcIds, CodeGenSymbols symbols)
        {
            if (Op == BinOp.Assign)
            {
                if (Left is not VarUseExpr)
                    throw new Exception("Left expression of an assignment should be a variable for the moment.");

                Right.CodeGen(func, funcIds, symbols);
                func.LastBlock.Instructions.Add(Instruction.CreateLocalSet((UInt16)symbols.VarIds[((VarUseExpr)Left).Var]));
                return;
            }

            Left.CodeGen(func, funcIds, symbols);
            Right.CodeGen(func, funcIds, symbols);
            func.LastBlock.Instructions.Add(new Instruction(GetOpCode()));
        }
    }
}
