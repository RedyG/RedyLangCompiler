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
        // Br i16
        // BrTrue i16
        // BrFalse i16
        // Exit,

        Call = 0x06, // u16
        // Ret
        Pop = 0x08,
        LocalGet, // u16
        LocalSet, // u16

        I8Const = 0x0C, // i8
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

        I8Load,
        I16Load,
        I32Load,
        I64Load,
        I8Store,
        I16Store,
        I32Store,
        I64Store,

        Alloca,
        AllocaPop,
        GcMalloc,
    }

    public class InvalidInstructionException : Exception
    {
        public InvalidInstructionException(string opCode) : base("Invalid instruction in final bytecode: " + opCode)
        {
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct Instruction
    {
        [FieldOffset(0)] public readonly OpCode OpCode;

        [FieldOffset(8)] public readonly sbyte i8;
        [FieldOffset(8)] public readonly Int16 i16;
        [FieldOffset(8)] public readonly Int32 i32;
        [FieldOffset(8)] public readonly Int64 i64;
        [FieldOffset(8)] public readonly UInt16 u16;

        [FieldOffset(16)] public readonly Func? func = null;




        public Instruction(OpCode opCode)
        {
            OpCode = opCode;
        }

        public Instruction(OpCode opCode, Func func)
        {
            OpCode = opCode;
            this.func = func;
        }

        private Instruction(OpCode opCode, sbyte i8)
        {
            OpCode = opCode;
            this.i8 = i8;
        }

        private Instruction(OpCode opCode, Int16 i16)
        {
            OpCode = opCode;
            this.i16 = i16;
        }

        private Instruction(OpCode opCode, Int32 i32)
        {
            OpCode = opCode;
            this.i32 = i32;
        }

        private Instruction(OpCode opCode, Int64 i64)
        {
            OpCode = opCode;
            this.i64 = i64;
        }

        private Instruction(OpCode opCode, UInt16 u16)
        {
            OpCode = opCode;
            this.u16 = u16;
        }

        public static Instruction CreateCall(Func func) => new Instruction(OpCode.Call, func);
        public static Instruction CreateLocalGet(UInt16 id) => new Instruction(OpCode.LocalGet, id);
        public static Instruction CreateLocalSet(UInt16 id) => new Instruction(OpCode.LocalSet, id);
        public static Instruction CreateI8Const(sbyte value) => new Instruction(OpCode.I8Const, value);
        public static Instruction CreateI16Const(Int16 value) => new Instruction(OpCode.I16Const, value);
        public static Instruction CreateI32Const(Int32 value) => new Instruction(OpCode.I32Const, value);
        public static Instruction CreateI64Const(Int64 value) => new Instruction(OpCode.I64Const, value);

        public static Instruction CreateI8Load(Int32 offset) => new Instruction(OpCode.I8Load, offset);
        public static Instruction CreateI16Load(Int32 offset) => new Instruction(OpCode.I16Load, offset);
        public static Instruction CreateI32Load(Int32 offset) => new Instruction(OpCode.I32Load, offset);
        public static Instruction CreateI64Load(Int32 offset) => new Instruction(OpCode.I64Load, offset);
        public static Instruction CreateI8Store(Int32 offset) => new Instruction(OpCode.I8Store, offset);
        public static Instruction CreateI16Store(Int32 offset) => new Instruction(OpCode.I16Store, offset);
        public static Instruction CreateI32Store(Int32 offset) => new Instruction(OpCode.I32Store, offset);
        public static Instruction CreateI64Store(Int32 offset) => new Instruction(OpCode.I64Store, offset);

        public static Instruction CreateAlloca(Int32 size) => new Instruction(OpCode.Alloca, size);
        public static Instruction CreateAllocaPop(Int32 size) => new Instruction(OpCode.AllocaPop, size);
        public static Instruction CreateGcMalloc(Int32 size) => new Instruction(OpCode.GcMalloc, size);

        public void WriteTo(Module module, ByteList list)
        {
            list.Add((byte)OpCode);
            switch (OpCode)
            {
                case OpCode.Call:
                    list.Add((UInt16)module.GetFuncId(func!));
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

                case OpCode.I8Load:
                case OpCode.I16Load:
                case OpCode.I32Load:
                case OpCode.I64Load:
                case OpCode.I8Store:
                case OpCode.I16Store:    
                case OpCode.I32Store:
                case OpCode.I64Store:
                case OpCode.Alloca:
                case OpCode.AllocaPop:
                case OpCode.GcMalloc:
                    list.Add(i32);
                    break;

            }
        }
    }
}