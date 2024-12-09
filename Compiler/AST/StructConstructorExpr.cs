using Compiler.ByteCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.AST
{
    internal class StructConstructorExpr : IExpr
    {
        public IType Type { get; }
        public List<(Field Field, IExpr Value)> Args { get; }

        public StructConstructorExpr(IType type, List<(Field Field, IExpr Value)> args)
        {
            Type = type;
            Args = args;
        }

        // TODO: optimize for single field struct or less than 64 bits
        public void CodeGen(ByteCode.Func func, Dictionary<Func, ByteCode.Func> funcs, CodeGenSymbols symbols)
        {
            func.LastBlock.Instructions.Add(Instruction.CreateAlloca(Type.Size()));
            foreach (var (field, value) in Args)
            {
                value.CodeGen(func, funcs, symbols);
                func.LastBlock.Instructions.Add(Instruction.CreateStore(value.Type.Size(), (Int32)field.Offset()));
            }
        }
    }
}
