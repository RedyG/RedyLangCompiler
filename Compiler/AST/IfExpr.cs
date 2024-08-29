using Compiler.ByteCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.AST
{
    public class IfExpr : IExpr
    {
        public Type Type { get; }
        public IExpr Condition { get; }
        public IExpr Then { get; }
        public IExpr? Else { get; }

        public IfExpr(Type type, IExpr condition, IExpr then, IExpr? @else = null)
        {
            Type = type;
            Condition = condition;
            Then = then;
            Else = @else;
        }

        public void CodeGen(ByteCode.Func func, Dictionary<Func, int> funcIds, CodeGenSymbols symbols)
        {
            Condition.CodeGen(func, funcIds, symbols);
            var conditionBlock = func.LastBlock;

            func.AddBlock();
            Then.CodeGen(func, funcIds, symbols);
            var thenBlock = func.LastBlock;

            Block? elseBlock = null;
            if (Else != null)
            {
                func.AddBlock();
                Else.CodeGen(func, funcIds, symbols);
                elseBlock = func.LastBlock;
            }

            Block? mergeBlock = func.AddBlock();
            if (thenBlock.BrInstruction == null)
                thenBlock.BrInstruction = BrInstruction.CreateBr(mergeBlock);

            if (elseBlock != null && elseBlock.BrInstruction == null)
                elseBlock.BrInstruction = BrInstruction.CreateBr(mergeBlock);

            if (thenBlock.BrInstruction != null && elseBlock?.BrInstruction != null)
                func.Blocks.RemoveAt(func.Blocks.Count - 1);

            conditionBlock.BrInstruction = BrInstruction.CreateBrIf(thenBlock, elseBlock == null ? mergeBlock : elseBlock);
        }
    }
}
