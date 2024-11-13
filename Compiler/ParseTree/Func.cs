using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.ParseTree
{
    public class Func : Decl
    {
        public Proto Proto { get; }
        public IExpr Body { get; }

        public ModuleFile ModuleFile => Proto.ModuleFile;

        public Func(Proto proto, IExpr body)
        {
            Proto = proto;
            Body = body;
        }

        public AST.Func? Register(GlobalSymbols globals, AST.ModuleFile? moduleFile)
        {
            if (globals.FuncsAST.TryGetValue(this, out var func))
                return func;

            var proto = Proto.Register(this, globals, moduleFile);
            if (proto == null)
                return null; 

            func = new AST.Func(proto, null, Proto.Identifier.Name == "main");
            globals.FuncsAST.Add(this, func);
            return func;
        }

        public AST.Func? ToAST(GlobalSymbols globals, AST.Func func)
        {
            var scopedSymbols = new ScopedSymbols();
            var proto = Proto.ToAST(this, globals, scopedSymbols, func.Proto);
            if (proto == null)
                return null;

            var body = Body.ToAST(this, globals, scopedSymbols);
            if (body == null)
                return null;
            func.Body = body;
            return func;
        }
    }
}


