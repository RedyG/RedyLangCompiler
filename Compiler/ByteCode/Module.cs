using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.ByteCode
{
    public struct ImportedFunc
    {
        public string FileName { get; set; }

    }

    public class Module
    {
        public List<string> ImportedFuncs { get; } = new();
        public List<Func> Funcs { get; } = new();

        public void WriteTo(ByteList list)
        {
            list.Add("redylang"); // magic number
            list.Add((Int64)0); // version

            list.Add((UInt16)Funcs.Count);
            list.Add((UInt16)ImportedFuncs.Count);

            foreach (var func in ImportedFuncs)
            {
                list.Add(func + "\0");
            }

            list.AdvanceBy(Funcs.Count * 8);

            int funcOffset = 0;

            for (int i = 0; i < Funcs.Count; i++)
            {
                var func = Funcs[i];
                int start = i * 8 + 18;
                list.WriteAt(start, (UInt16)func.ParamsCount);
                list.WriteAt(start + 2, (UInt16)func.LocalsCount);
                list.WriteAt(start + 4, funcOffset);

                int count = list.Count;
                func.WriteTo(list);
                funcOffset += (list.Count - count);
            }

        }

        public Module(List<string> importedFuncs)
        {
            ImportedFuncs = importedFuncs;
        }

        public Module(List<Func> funcs, List<string> importedFuncs)
        {
            Funcs = funcs;
            ImportedFuncs = importedFuncs;
        }
    }
}
