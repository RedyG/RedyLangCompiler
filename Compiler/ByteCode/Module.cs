using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.ByteCode
{
    /* format:
     *
     * 
     * # header:
     * redylang
     * uint64 version
     * 
     * uint16 importedFuncCount
     * uint16 funcCount
     * uint32 constantPoolSize
     * 
     * # imports:
     * uint16 funcId
     * String path // relative to root, no extension
     * 
     * # funcs:
     * ...
     * 
     * # constant pool:
     * ...
     * 
     */

    /* String:
     * uint32 length
     * char [length]
     */

    public class Module
    {
        private const int ProtocolFuncSize = 12;
        public string FileName { get; set; }
        public List<Func> ImportedFuncs { get; set; } = new();
        public List<Func> Funcs { get; set; } = new();

        private ByteList constPool = new();

        public void WriteTo(ByteList list, string root)
        {
            list.Add("redylang"); // magic number
            list.Add((UInt64)0); // version

            list.Add((UInt16)ImportedFuncs.Count);
            list.Add((UInt16)Funcs.Count);
            list.Add((UInt32)constPool.Count);

            foreach (var func in ImportedFuncs)
            {
                list.Add((UInt16)func.GetId());
                var path = Path.GetRelativePath(root, func.Module!.FileName).Replace("\\", "/").Split(".")[0];
                list.AddRString(path); // TODO: nested not just file name
            }

            int funcsStart = list.Count;
            list.AdvanceBy(Funcs.Count * ProtocolFuncSize);

            list.Add(constPool);

            int funcOffset = 0;
            for (int i = 0; i < Funcs.Count; i++)
            {
                var func = Funcs[i];
                int start = i * ProtocolFuncSize + funcsStart;

                list.WriteAt(start, (UInt16)func.ParamsCount);
                list.WriteAt(start + 2, (UInt16)func.LocalsCount);
                list.WriteAt(start + 8, (UInt32)funcOffset);

                int previousCount = list.Count;
                func.WriteTo(this, list);
                int newCount = (list.Count - previousCount);
                list.WriteAt(start + 4, (UInt32)newCount);
                funcOffset += newCount;
            }

        }

        public int GetFuncId(Func func)
        {
            var importedId = ImportedFuncs.IndexOf(func);
            if (importedId != -1)
                return importedId;

            var id = Funcs.IndexOf(func);
            if (id != -1)
                return id + ImportedFuncs.Count;

            return -1;
        }

        public uint AddConst(Action<ByteList> writeFn)
        {
            var offset = (uint)constPool.Count;
            writeFn(constPool);
            return offset;
        }

        public Module(string fileName, List<Func> importedFuncs)
        {
            FileName = fileName;
            ImportedFuncs = importedFuncs;
        }

        public Module(string fileName, List<Func> importedFuncs, List<Func> funcs)
        {
            FileName = fileName;
            Funcs = funcs;
            ImportedFuncs = importedFuncs;
        }
    }
}
