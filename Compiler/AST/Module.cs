using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.AST
{
    public class Module
    {
        public List<Func> Funcs { get; set; } = new();

        public ByteCode.Module CodeGen()
        {
            var module = new ByteCode.Module();
            var funcIds = new Dictionary<Func, int>();
            for (int i = 0; i < Funcs.Count; i++)
                funcIds.Add(Funcs[i], i);

            foreach (var func in Funcs)
                module.Funcs.Add(func.CodeGen(funcIds));

            module.Funcs[0].LastBlock.BrInstruction = ByteCode.BrInstruction.CreateExit();

            return module;
        }
    }
}
