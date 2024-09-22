using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.ParseTree
{
    public class Func
    {
        public Proto Proto { get; }
        public IExpr Body { get; }

        public ModuleFile ModuleFile { get; }

        public Func(ModuleFile moduleFile, Proto proto, IExpr body)
        {
            ModuleFile = moduleFile;
            Proto = proto;
            Body = body;
        }

        public AST.Func? ToAST(GlobalSymbols globals)
        {
            if (globals.FuncsAST.TryGetValue(this, out var funcAST))
                return funcAST;

            var scopedSymbols = new ScopedSymbols();

            var proto = Proto.ToAST(this, globals, scopedSymbols);
            if (proto == null)
                return null;

            var func = new AST.Func(proto, null);
            globals.FuncsAST[this] = func;

            var body = Body.ToAST(this, globals, scopedSymbols);
            if (body == null)
                return null;

            func.Body = body;
            return func;
        }
    }
}


