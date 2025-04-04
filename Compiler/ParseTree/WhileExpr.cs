using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.ParseTree
{
    public class WhileExpr : IExpr
    {
        public TextRange Range { get; set; }
        public IExpr Condition { get; }
        public IExpr Body { get; }

        public WhileExpr(TextRange range, IExpr condition, IExpr body)
        {
            Range = range;
            Condition = condition;
            Body = body;
        }

        public AST.IExpr? ToAST(Decl decl, GlobalSymbols globals, ScopedSymbols scopedSymbols, bool ignored = false)
        {
            bool failed = false;

            var condition = Condition.ToAST(decl, globals, scopedSymbols);
            if (condition == null)
                failed = true;
            else if (condition.Type is not AST.IType.Bool)
            {
                Logger.MismatchedTypesWhileCond(decl.ModuleFile, condition.Type, this);
                failed = true;
            }

            var body = Body.ToAST(decl, globals, scopedSymbols);
            if (failed || body == null)
                return null;

            if (!body.Type.IsEmpty)
            {
                Logger.MismatchedTypesWhileBody(decl.ModuleFile, body.Type, this);
                return null;
            }

            return new AST.WhileExpr(condition!, body);
        }
    }
}
