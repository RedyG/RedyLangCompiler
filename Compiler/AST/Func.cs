using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.AST
{
    public class Func
    {
        public Proto Proto { get; set; }
        public IExpr? Body { get; set; }

        public ByteCode.Func CodeGen(Dictionary<Func, int> funcIds)
        {
            var func = new ByteCode.Func(Proto.Params.Count, 0);
            var symbols = new CodeGenSymbols();
            foreach (var param in Proto.Params)
            {
                symbols.AddVar(param);
                func.LocalsCount++;
            }

            func.AddBlock();
            Body!.CodeGen(func, funcIds, symbols);
            return func;
        }

        public Func(Proto proto, IExpr? body)
        {
            Proto = proto;
            Body = body;
        }
    }
}
