using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.ParseTree
{
    public class CallExpr : IExpr
    {
        public TextRange Range { get; }
        public IExpr Callee { get; }
        public List<IExpr> Args { get; }

        public CallExpr(TextRange range, IExpr callee, List<IExpr> args)
        {
            Range = range;
            Callee = callee;
            Args = args;
        }

        public AST.IExpr? ToAST(Func func, GlobalSymbols globals, ScopedSymbols scopedSymbols, bool ignored = false)
        {
            if (Callee is Identifier identifier)
            {
                var callee = func.ModuleFile.Module.GetFunc(identifier);
                if (callee == null)
                {
                    Logger.FuncNotFound(func.ModuleFile, identifier);
                    return null;
                }

                if (callee.Proto.Params.Count != Args.Count)
                {
                    Logger.InvalidArgsCount(func.ModuleFile, callee.Proto.Params.Count, this);
                    return null;
                }

                var calleeAST = callee.ToAST(globals);
                if (calleeAST == null)
                    return null;

                var args = Args.Select(arg => arg.ToAST(func, globals, scopedSymbols)).ToList();

                if (args.Any(arg => arg == null))
                    return null;

                return new AST.CallExpr(calleeAST, args);
            }

            return null; // fpr the moment
        }
    }
}
