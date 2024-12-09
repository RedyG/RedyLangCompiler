using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.ParseTree
{
    public class IfExpr : IExpr
    {
        public IExpr Condition { get; }
        public IExpr Then { get; }
        public IExpr? Else { get; }

        public TextRange Range { get; }

        public IfExpr(TextRange range, IExpr condition, IExpr then, IExpr? @else = null)
        {
            Range = range;
            Condition = condition;
            Then = then;
            Else = @else;
        }

        public AST.IExpr? ToAST(Decl decl, GlobalSymbols globals, ScopedSymbols scopedSymbols, bool ignored = false)
        {
            bool failed = false;

            var condition = Condition.ToAST(decl, globals, scopedSymbols);
            if (condition == null)
                failed = true;
            else if (condition.Type is not AST.IType.Bool)
            {
                Logger.MismatchedTypesIf(decl.ModuleFile, condition.Type, this);
                failed = true;
            }

            var then = Then.ToAST(decl, globals, scopedSymbols);
            if (then == null)
                failed = true;

            if (Else == null)
            {
                if (then != null && !then.Type.IsEmpty)
                {
                    Logger.MismatchedTypesNoElse(decl.ModuleFile, then.Type, this);
                    failed = true;
                }
                if (failed)
                    return null;

                return new AST.IfExpr(then!.Type, condition!, then);
            }

            var @else = Else.ToAST(decl, globals, scopedSymbols);
            if (@else == null || failed)
                return null;

            if (!then!.Type.Equals(@else.Type))
            {
                Logger.MismatchedTypesIfElse(decl.ModuleFile, then.Type, @else.Type, this);
                return null;
            }

            return new AST.IfExpr(then.Type, condition!, then, @else);
        }
    }
}
