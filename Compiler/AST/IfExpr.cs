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

        public void CodeGen(ByteCode.Func func, Dictionary<Func, ByteCode.Func> funcs, CodeGenSymbols symbols)
        {
            Condition.CodeGen(func, funcs, symbols);
            var conditionBlock = func.LastBlock;

            func.AddBlock();
            Then.CodeGen(func, funcs, symbols);
            var thenBlock = func.LastBlock;

            Block? elseBlock = null;
            if (Else != null)
            {
                func.AddBlock();
                Else.CodeGen(func, funcs, symbols);
                elseBlock = func.LastBlock;
            }


            // holy shit, basically not adding the merging stuff if the blocks have their branching of their own already ( like ret )
            if ((thenBlock.BrInstruction != null && elseBlock == null) || (thenBlock.BrInstruction != null && elseBlock != null && elseBlock.BrInstruction != null))
            {
                if (elseBlock == null)
                    elseBlock = func.AddBlock();
                conditionBlock.BrInstruction = BrInstruction.CreateBrIf(thenBlock, elseBlock);
                return;
            }

            Block? mergeBlock = func.AddBlock();
            if (thenBlock.BrInstruction == null)
                thenBlock.BrInstruction = BrInstruction.CreateBr(mergeBlock);

            if (elseBlock != null && elseBlock.BrInstruction == null)
                elseBlock.BrInstruction = BrInstruction.CreateBr(mergeBlock);

            conditionBlock.BrInstruction = BrInstruction.CreateBrIf(thenBlock, elseBlock == null ? mergeBlock : elseBlock);
        }
    }
}
