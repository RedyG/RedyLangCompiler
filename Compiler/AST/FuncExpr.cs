using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.AST
{
    public class FuncExpr : IExpr
    {
        public IType Type { get; }
        public Func Func { get; }

        public FuncExpr(IType type, Func func)
        {
            Type = type;
            Func = func;
        }

        public void CodeGen(ByteCode.Func func, Dictionary<Func, ByteCode.Func> funcs, CodeGenSymbols symbols)
        {
            throw new NotImplementedException(); // not implemented yet, need fn pointers and stuff
        }
    }
}
