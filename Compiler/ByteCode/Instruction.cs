using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.ByteCode
{
    public enum OpCode
    {
        Nop = 0x00,
        Br, // i16
        BrTrue, // i16
        BrFalse, // i16
        Exit,

        Call, // i16
        Ret,
        Pop,
        LocalGet, // u16
        LocalSet, // u16

        I8Const, // i8
        I16Const, // i16
        I32Const, // i32
        I64Const, // i64

        I32Eqz = 0x10,
        I32Eq,
        I32Ne,
        I32Lt,
        U32Lt,
        I32Gt,
        U32GT,
        I32Le,
        U32Le,
        I32Ge,
        U32Ge,

        I64Eq = 0x1C,
        I64Ne,
        I64Lt,
        U64Lt,
        I64Gt,
        U64GT,
        I64Le,
        U64Le,
        I64Ge,
        U64Ge,

        F32Eq,
        F32Ne,
        F32Lt,
        F32Gt,
        F32Le,
        F32Ge,

        F64Eq,
        F64Ne,
        F64Lt,
        F64Gt,
        F64Le,
        F64Ge,

        I32Add,
        I32Sub,
        I32Mul,
        I32Div,
        U32Div,
        I32Rem,
        U32Rem,
        I32And,
        I32Or,
        I32Xor,
        I32Shl,
        I32Shr,
        U32Shr,

        I64Add,
        I64Sub,
        I64Mul,
        I64Div,
        U64Div,
        I64Rem,
        U64Rem,
        I64And,
        I64Or,
        I64Xor,
        I64Shl,
        I64Shr,
        U64Shr,

        F32Add,
        F32Sub,
        F32Mul,
        F32Div,

        F64Add,
        F64Sub,
        F64Mul,
        F64Div,

    }

    [StructLayout(LayoutKind.Explicit)]
    public struct Instruction
    {
        [FieldOffset(0)] public readonly OpCode OpCode;

        [FieldOffset(8)] public readonly sbyte i8;
        [FieldOffset(8)] public readonly short i16;
        [FieldOffset(8)] public readonly int i32;
        [FieldOffset(8)] public readonly long i64;
        [FieldOffset(8)] public readonly ushort u16;

        public Instruction(OpCode opCode, sbyte i8)
        {
            OpCode = opCode;
            this.i8 = i8;
        }

        public Instruction(OpCode opCode, short i16)
        {
            OpCode = opCode;
            this.i16 = i16;
        }

        public Instruction(OpCode opCode, int i32)
        {
            OpCode = opCode;
            this.i32 = i32;
        }

        public Instruction(OpCode opCode, long i64)
        {
            OpCode = opCode;
            this.i64 = i64;
        }

        public Instruction(OpCode opCode, ushort u16)
        {
            OpCode = opCode;
            this.u16 = u16;
        }

        public void WriteTo(ByteList list)
        {
            list.Add((byte)OpCode);
            switch (OpCode)
            {
                case OpCode.Br:
                case OpCode.BrTrue:
                case OpCode.BrFalse:
                case OpCode.Call:
                    list.Add(i16);
                    break;

                case OpCode.LocalGet:
                case OpCode.LocalSet:
                    list.Add(u16);
                    break;

                case OpCode.I8Const:
                    list.Add(i8);
                    break;
                case OpCode.I16Const:
                    list.Add(i16);
                    break;
                case OpCode.I32Const:
                    list.Add(i32);
                    break;
                case OpCode.I64Const:
                    list.Add(i64);
                    break;
            }
        }


    }
}
