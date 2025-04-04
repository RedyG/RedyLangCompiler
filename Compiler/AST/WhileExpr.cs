using Compiler.ByteCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.AST
{
    public class WhileExpr : IExpr
    {
        public IType Type => new IType.Void();
        public IExpr Condition { get; }
        public IExpr Body { get; }

        public WhileExpr(IExpr condition, IExpr body)
        {
            Condition = condition;
            Body = body;
        }

        public void CodeGen(ByteCode.Func func, Dictionary<Func, ByteCode.Func> funcs, CodeGenSymbols symbols)
        {
            var conditionBlock = func.AddBlock();
            Condition.CodeGen(func, funcs, symbols);

            var bodyBlock = func.AddBlock();
            Body.CodeGen(func, funcs, symbols);
            bodyBlock.BrInstruction = BrInstruction.CreateBr(conditionBlock);

            func.AddBlock();
            conditionBlock.BrInstruction = BrInstruction.CreateBrIf(bodyBlock, func.LastBlock);

        }
    }
}
