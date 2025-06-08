
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.AST
{
    public class Func
    {
        public bool Main { get; set; }
        public Proto Proto { get; set; }
        public IExpr? Body { get; set; }

        public ByteCode.Func CodeGen(Dictionary<Func, ByteCode.Func> funcs, ByteCode.Module? module = null)
        {
            if (Body == null)
                throw new InvalidOperationException("Function body cannot be null.");

            if (funcs.TryGetValue(this, out var func))
                return func;

            func = new ByteCode.Func(Proto.Params.Count, 0);
            func.Module = module;
            var symbols = new CodeGenSymbols();
            foreach (var param in Proto.Params)
            {
                symbols.AddVar(param);
            }

            if (Proto.ReturnType.ToConcrete() is IType.Struct @struct)
            {
                func.ParamsCount++;
                symbols.AddVar(new VarDeclStatement(@struct));
            }

            funcs.Add(this, func);

            func.AddBlock();
            Body!.CodeGen(func, funcs, symbols);

            if (Main)
                func.LastBlock.BrInstruction = ByteCode.BrInstruction.CreateExit();

            if (func.LastBlock.BrInstruction == null)
            {
                if (Proto.ReturnType.IsEmpty)
                    func.LastBlock.BrInstruction = ByteCode.BrInstruction.CreateRetVoid();
                else
                    func.LastBlock.BrInstruction = ByteCode.BrInstruction.CreateRet();
            }

            return func;
        }

        public Func(Proto proto, IExpr? body, bool main = false)
        {
            Proto = proto;
            Body = body;
            Main = main;
        }
    }
}
