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

        private Instruction GetInstruction() => (Type, Op) switch
        {
            (IType.I32, BinOp.Mul) => new Instruction(OpCode.I32Mul),
            (IType.I32, BinOp.Div) => new Instruction(OpCode.I32Div),
            (IType.I32, BinOp.Add) => new Instruction(OpCode.I32Add),
            (IType.I32, BinOp.Sub) => new Instruction(OpCode.I32Sub),
            (IType.Bool, BinOp.Lt) => new Instruction(OpCode.I32Lt),
            (IType.Bool, BinOp.Le) => new Instruction(OpCode.I32Le),
            (IType.Bool, BinOp.Gt) => new Instruction(OpCode.I32Gt),
            (IType.Bool, BinOp.Ge) => new Instruction(OpCode.I32Ge),
            (IType.String, BinOp.Add) => Instruction.CreateCallIntrinsic(Intrinsic.StringConcat),

            (_, _) => throw new NotImplementedException()
        };

        public void CodeGen(ByteCode.Func func, Dictionary<Func, ByteCode.Func> funcs, CodeGenSymbols symbols)
        {
            if (Op == BinOp.Assign)
            {
                if (Left is VarUseExpr varUse)
                {
                    Right.CodeGen(func, funcs, symbols);
                    func.LastBlock.Instructions.Add(Instruction.CreateLocalSet((UInt16)symbols.VarIds[((VarUseExpr)Left).Var]));
                }
                else if (Left is DeRefExpr deRef)
                {
                    deRef.Expr.CodeGen(func, funcs, symbols);
                    Right.CodeGen(func, funcs, symbols);
                    func.LastBlock.Instructions.Add(Instruction.CreateStore(deRef.Type.Size(), 0));
                    func.LastBlock.Instructions.Add(new Instruction(OpCode.Pop));
                }
                else if (Left is AccessExpr access)
                {
                    access.LValue.CodeGen(func, funcs, symbols);
                    Right.CodeGen(func, funcs, symbols);
                    func.LastBlock.Instructions.Add(Instruction.CreateMemCpy((Int32)access.Field.Offset(), 0, access.Field.Type.Size())); 
                }
                return;
            }

            Left.CodeGen(func, funcs, symbols);
            Right.CodeGen(func, funcs, symbols);
            func.LastBlock.Instructions.Add(GetInstruction());
        }
    }
}
