using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.ByteCode
{
    public class Module
    {
        public List<Func> Funcs { get; } = new();

        public void WriteTo(ByteList list)
        {
            list.Add("redylang"); // magic number
            list.Add((Int64)0); // version
            list.Add((Int16)Funcs.Count);
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
    }
}
