using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compiler.ByteCode;

namespace Compiler.AST
{
    public class VarDeclStatement : IStatement
    {
        public Type Type { get; }
        public IExpr? Value { get; set; }

        public VarDeclStatement(Type type, IExpr? value = null)
        {
            Type = type;
            Value = value;
        }

        public void CodeGen(ByteCode.Func func, Dictionary<Func, ByteCode.Func> funcs, CodeGenSymbols symbols)
        {
            if (Value != null)
            {
                Value.CodeGen(func, funcs, symbols);
                func.LastBlock.Instructions.Add(Instruction.CreateLocalSet((UInt16)symbols.CurrentVarId));
            }

            func.LocalsCount++;
            symbols.AddVar(this);
        }
    }
}
