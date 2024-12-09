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
        public IType Type { get; }
        public IExpr? Value { get; set; }
        public List<RefExpr> Refs { get; set; } = new();

        public VarDeclStatement(IType type, IExpr? value = null)
        {
            Type = type;
            Value = value;
        }

        public void CodeGen(ByteCode.Func func, Dictionary<Func, ByteCode.Func> funcs, CodeGenSymbols symbols)
        {
            if (Value != null)
            {
                var alloca = Refs.Count > 0 && Value is not IType.Struct;
                if (alloca)
                    func.LastBlock.Instructions.Add(Instruction.CreateAlloca(Type.Size()));
 
                Value.CodeGen(func, funcs, symbols);

                if (alloca)
                    func.LastBlock.Instructions.Add(Instruction.CreateStore(Type.Size(), 0));

                func.LastBlock.Instructions.Add(Instruction.CreateLocalSet((UInt16)symbols.CurrentVarId));
            }

            func.LocalsCount++;
            symbols.AddVar(this);
        }
    }

    public class Param : VarDeclStatement
    {
        private bool _hasDefaultValue;
        public bool HasDefaultValue => _hasDefaultValue || Value != null;
        public Param(IType type, IExpr? value = null, bool hasDefaultValue = false) : base(type, value)
        {
        }
    }
}
