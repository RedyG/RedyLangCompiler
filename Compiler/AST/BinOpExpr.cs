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
        public IType Type { get; }
        public IExpr Left { get; }
        public BinOp Op { get; }
        public IExpr Right { get; }

        public BinOpExpr(IType type, IExpr left, BinOp op, IExpr right)
        {
            Type = type;
            Left = left;
            Op = op;
            Right = right;
        }

        private OpCode GetOpCode() => (Type, Op) switch
        {
            (IType.I32, BinOp.Mul) => OpCode.I32Mul,
            (IType.I32, BinOp.Div) => OpCode.I32Div,
            (IType.I32, BinOp.Add) => OpCode.I32Add,
            (IType.I32, BinOp.Sub) => OpCode.I32Sub,
            (IType.Bool, BinOp.Lt) => OpCode.I32Lt,
            (IType.Bool, BinOp.Le) => OpCode.I32Le,
            (IType.Bool, BinOp.Gt) => OpCode.I32Gt,
            (IType.Bool, BinOp.Ge) => OpCode.I32Ge,

            (_, _) => throw new NotImplementedException()
        };

        public void CodeGen(ByteCode.Func func, Dictionary<Func, ByteCode.Func> funcs, CodeGenSymbols symbols)
        {
            if (Op == BinOp.Assign)
            {
                if (Left is not VarUseExpr)
                    throw new Exception("Left expression of an assignment should be a variable for the moment.");

                Right.CodeGen(func, funcs, symbols);
                func.LastBlock.Instructions.Add(Instruction.CreateLocalSet((UInt16)symbols.VarIds[((VarUseExpr)Left).Var]));
                return;
            }

            Left.CodeGen(func, funcs, symbols);
            Right.CodeGen(func, funcs, symbols);
            func.LastBlock.Instructions.Add(new Instruction(GetOpCode()));
        }
    }
}
