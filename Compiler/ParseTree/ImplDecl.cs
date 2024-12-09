using Compiler.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.ParseTree
{
    public class ImplDecl : Decl
    {
        public Type? Trait { get; }
        public Type Type { get; } 
        public List<Func> Funcs { get; }

        public ModuleFile ModuleFile { get; }

        public ImplDecl(ModuleFile moduleFile, Type? trait, Type type, List<Func> funcs)
        {
            ModuleFile = moduleFile;
            Trait = trait;
            Type = type;
            Funcs = funcs;
        }

        public AST.Impl? Register(GlobalSymbols globals, AST.ModuleFile module)
        {
            var type = Type.ToAST(this, globals, new());
            if (type == null)
            {
                Logger.TypeNotFound(ModuleFile, ((Type.Identifier)Type).Identifer);
                return null;
            }

            IType? trait = null;
            if (Trait != null)
            {
                trait = Trait.ToAST(this, globals, new());
                if (trait == null)
                {
                    Logger.TypeNotFound(ModuleFile, ((Type.Identifier)Trait).Identifer);
                    return null;
                }
            }

            var funcs = Funcs.Select(func => func.Register(globals, module)).ToList();

            if (funcs.Any(func => func == null))
                return null;

            return new AST.Impl(trait, type, funcs!, module);
        }
    }
}
