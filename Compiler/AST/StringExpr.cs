using Compiler.ByteCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.AST
{
    public class StringExpr : IExpr
    {
        public IType Type { get; } = new IType.String();
        public string Content { get; }

        public void CodeGen(ByteCode.Func func, Dictionary<Func, ByteCode.Func> funcs, CodeGenSymbols symbols)
        {
            var offset = func.Module.AddConst(byteList => byteList.AddRString(Content));
            func.LastBlock.Instructions.Add(Instruction.CreatePtrLoadConst(offset));
        }

        public StringExpr(string content)
        {
            Content = content;
        }
    }
}
