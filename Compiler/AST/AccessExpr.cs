using Compiler.ByteCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.AST
{
    public class AccessExpr : IExpr
    {
        public Type Type { get; }

        public IExpr LValue { get; }
        public Field Field { get; }

        public AccessExpr(IExpr lValue, Field field)
        {
            Type = field.Type;
            LValue = lValue;
            Field = field;
        }


        public void CodeGen(ByteCode.Func func, Dictionary<Func, ByteCode.Func> funcs, CodeGenSymbols symbols)
        {
            LValue.CodeGen(func, funcs, symbols);
            switch (Field.Type)
            {
                case Type.Bool:
                    func.LastBlock.Instructions.Add(Instruction.CreateI8Load((int)Field.Offset()));
                    break;
                case Type.I32:
                    func.LastBlock.Instructions.Add(Instruction.CreateI32Load((int)Field.Offset()));
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
