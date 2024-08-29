using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.AST
{
    public class Func
    {
        public Type ReturnType { get; set; }
        public List<VarDeclStatement> Params { get; set; } = new();
        public IExpr Body { get; set; }

        public ByteCode.Func CodeGen(Dictionary<Func, int> funcIds)
        {
            var func = new ByteCode.Func(Params.Count, 0);
            var symbols = new CodeGenSymbols();
            foreach (var param in Params)
            {
                symbols.AddVar(param);
                func.LocalsCount++;
            }

            func.AddBlock();
            Body.CodeGen(func, funcIds, symbols);
            return func;
        }

        public Func(Type returnType, IExpr body, List<VarDeclStatement> @params)
        {
            ReturnType = returnType;
            Body = body;
            Params = @params;
        }

        public Func(Type returnType, IExpr body)
        {
            ReturnType = returnType;
            Body = body;
        }
    }
}
