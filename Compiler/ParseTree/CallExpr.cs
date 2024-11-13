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

        public AST.IExpr? ToAST(Decl decl, GlobalSymbols globals, ScopedSymbols scopedSymbols, bool ignored = false)
        {
            if (Callee is Identifier identifier)
            {
                var callee = decl.ModuleFile.Module.GetFunc(identifier);
                if (callee == null)
                {
                    Logger.FuncNotFound(decl.ModuleFile, identifier);
                    return null;
                }

                if (callee.Proto.Params.Count != Args.Count)
                {
                    Logger.InvalidArgsCount(decl.ModuleFile, callee.Proto.Params.Count, this);
                    return null;
                }

                var calleeAST = globals.FuncsAST[callee];
                if (calleeAST == null)
                    return null;

                var args = Args.Select(arg => arg.ToAST(decl, globals, scopedSymbols)).ToList();

                if (args.Any(arg => arg == null))
                    return null;

                return new AST.CallExpr(calleeAST, args);
            }
            if (Callee is BinOpExpr binOpExpr && binOpExpr.OpNode.Op == BinOp.Access && binOpExpr.Right is Identifier method)
            {
                var self = binOpExpr.Left.ToAST(decl, globals, scopedSymbols);
                if (self == null)
                    return null;

                var methodAST = globals.Project.GetMethod(self.Type, method.Name.ToString());
                if (methodAST == null)
                {
                    Logger.MethodNotFound(decl.ModuleFile, self.Type, method);
                    return null;
                }

                if (methodAST.Proto.Params.Count != Args.Count + 1)
                {
                    Logger.InvalidArgsCount(decl.ModuleFile, methodAST.Proto.Params.Count, this);
                    return null;
                }


                List<AST.IExpr?> args = [self, ..Args.Select(arg => arg.ToAST(decl, globals, scopedSymbols))];

                if (args.Any(arg => arg == null))
                    return null;

                return new AST.CallExpr(methodAST, args);
            }

            return null; // for the moment
        }
    }
}
